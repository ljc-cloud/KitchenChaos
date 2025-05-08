using System;
using Counters;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {
    
    // public static Player Instance { get; private set; }

    public int PlayerId { get; private set; }

    public event EventHandler<Vector3> OnPickedSomething;
    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }


    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;


    private bool isWalking;
    private Vector3 lastInteractDir;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;
    private Vector2 moveVector;
    
    // 网络同步
    private Entity _mEntity;

    private void Awake()
    {
        _mEntity = GetComponent<Entity>();
    }

    private void Start() {
        // if (_mEntity.IsLocal)
        // {
        //     GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        //     GameInput.Instance.OnInteractAlternateAction += GameInput_OnInteractAlternateAction;
        // }
        // else
        // {
        //     _mEntity.OnPlayerInputChanged += Entity_OnPlayerInputChanged;
        // }
        _mEntity.OnPlayerInputChanged += Entity_OnPlayerInputChanged;
    }

    private void Entity_OnPlayerInputChanged(object sender, Entity.OnPlayerInputChangedEventArgs e)
    {
        switch (e.inputType)
        {
            case GameFrameSyncManager.PlayerInputType.None:
                break;
            case GameFrameSyncManager.PlayerInputType.Move:
                break;
            case GameFrameSyncManager.PlayerInputType.Interact:
                InteractAction();
                break;
            case GameFrameSyncManager.PlayerInputType.InteractAlt:
                InteractAlternateAction();
                break;
        }
    }

    private void GameInput_OnInteractAlternateAction(object sender, EventArgs e)
    {
        InteractAlternateAction();
    }

    private void InteractAlternateAction()
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;
        
        int counterId = _mEntity.interactCounterId;
        var counter = CounterManager.Instance.GetCounterFromId(counterId);
        counter?.InteractAlternate(this);
        // _mEntity.interactCounterId = -1;

        // if (selectedCounter != null) {
        //     // Debug.Log($"{selectedCounter.name}:InteractAlt!!!");
        //     selectedCounter.InteractAlternate(this);
        // }
    }

    private void GameInput_OnInteractAction(object sender, System.EventArgs e)
    {
        InteractAction();
    }

    private void InteractAction()
    {
        if (!KitchenGameManager.Instance.IsGamePlaying()) return;

        // if (_mEntity.IsLocal)
        // {
        //     if (selectedCounter != null) {
        //         // Debug.Log($"{selectedCounter.name}:Interact!!!");
        //         selectedCounter.Interact(this);
        //         // _mEntity.localInteractCounterId = selectedCounter.CounterId;
        //     }
        // }
        // else
        // {
        //     int counterId = _mEntity.interactCounterId;
        //     var counter = CounterManager.Instance.GetCounterFromId(counterId);
        //     counter?.Interact(this);
        // }
        int counterId = _mEntity.interactCounterId;
        var counter = CounterManager.Instance.GetCounterFromId(counterId);
        counter?.Interact(this);
        _mEntity.interactCounterId = -1;
    }

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions()
    {
        if (!_mEntity.IsLocal)
        {
            return;
        }
        Vector2 inputVector = GameInput.Instance.GetMovementVectorNormalized();

        Vector3 moveDir = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDir != Vector3.zero) {
            lastInteractDir = moveDir;
        }

        float interactDistance = 2f;
        if (Physics.Raycast(transform.position, lastInteractDir, out RaycastHit raycastHit, interactDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter))
            {
                _mEntity.localInteractCounterId = baseCounter.CounterId;
                // Has ClearCounter
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                }
            } else {
                SetSelectedCounter(null);
                _mEntity.localInteractCounterId = -1;
            }
        } else {
            SetSelectedCounter(null);
            _mEntity.localInteractCounterId = -1;
        }
    }

    private void HandleMovement() 
    {
        if (_mEntity.IsLocal)
        {
            moveVector = GameInput.Instance.GetMovementVectorNormalized();
        }
        else
        {
            moveVector = _mEntity.moveVector;
        }
        Vector3 moveDir = new Vector3(moveVector.x, 0f, moveVector.y);

        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;
        float playerHeight = 2f;
        bool canMove = !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDir, moveDistance);

        if (!canMove) {
            // Cannot move towards moveDir

            // Attempt only X movement
            Vector3 moveDirX = new Vector3(moveDir.x, 0, 0).normalized;
            canMove = (moveDir.x < -.5f || moveDir.x > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirX, moveDistance);

            if (canMove) {
                // Can move only on the X
                moveDir = moveDirX;
            } else {
                // Cannot move only on the X
                // Cannot move only on the X

                // Attempt only Z movement
                Vector3 moveDirZ = new Vector3(0, 0, moveDir.z).normalized;
                canMove = (moveDir.z < -.5f || moveDir.z > +.5f) && !Physics.CapsuleCast(transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirZ, moveDistance);

                if (canMove) {
                    // Can move only on the Z
                    moveDir = moveDirZ;
                } else {
                    // Cannot move in any direction
                }
            }
        }

        if (canMove) {
            transform.position += moveDir * moveDistance;
        }

        isWalking = moveDir != Vector3.zero;

        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
        
        // TODO: Lerp远端位置
        if (!_mEntity.IsLocal)
        {
            transform.position = Vector3.Lerp(transform.position, _mEntity.playerPosition
                , Time.deltaTime * 0.5f);
        }
    }

    private void SetSelectedCounter(BaseCounter counter) {
        // if (counter is null)
        // {
        //     return;
        // }
        this.selectedCounter = counter;
        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = counter
        });
    }

    public void SetPlayerId(int playerId)
    {
        Debug.Log("PlayerID: " + playerId);
        PlayerId = playerId;
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null) {
            OnPickedSomething?.Invoke(this, transform.position);
        }
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }

}