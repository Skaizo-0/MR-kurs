using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.InputSystem;  // Подключаем Input System

namespace Oculus.Interaction
{
    public class SnapInteractable2 : Interactable<SnapInteractor, SnapInteractable>, IRigidbodyRef
    {
        [SerializeField]
        private Rigidbody _rigidbody;
        public Rigidbody Rigidbody => _rigidbody;

        [FormerlySerializedAs("_snapPosesProvider")]
        [FormerlySerializedAs("_posesProvider")]
        [SerializeField, Optional, Interface(typeof(ISnapPoseDelegate))]
        private UnityEngine.Object _snapPoseDelegate;
        private ISnapPoseDelegate SnapPoseDelegate { get; set; }

        [SerializeField, Optional, Interface(typeof(IMovementProvider))]
        private UnityEngine.Object _movementProvider;
        private IMovementProvider MovementProvider { get; set; }

        private static CollisionInteractionRegistry<SnapInteractor, SnapInteractable> _registry = null;

        private InputAction moveAction;
        private InputAction buttonAction;

        private InputActionMap inputActionMap; // InputActionMap для обработки действий

        #region Editor events
        private void Reset()
        {
            _rigidbody = this.GetComponentInParent<Rigidbody>();
        }
        #endregion

        protected override void Awake()
        {
            base.Awake();
            MovementProvider = _movementProvider as IMovementProvider;
            SnapPoseDelegate = _snapPoseDelegate as ISnapPoseDelegate;

            // Инициализация InputActionAsset
            var inputActionAsset = new InputActionAsset();

            // Создаем новый InputActionMap
            inputActionMap = inputActionAsset.AddActionMap("PlayerActions");

            // Добавляем действия в этот InputActionMap
            moveAction = inputActionMap.AddAction("Move", binding: "<Gamepad>/leftStick");
            buttonAction = inputActionMap.AddAction("ButtonPress", binding: "<XRController>/button1");

            // Включаем действия
            moveAction.Enable();
            buttonAction.Enable();
        }

        protected override void Start()
        {
            this.BeginStart(ref _started, () => base.Start());
            this.AssertField(Rigidbody, nameof(Rigidbody));
            if (_registry == null)
            {
                _registry = new CollisionInteractionRegistry<SnapInteractor, SnapInteractable>();
                SetRegistry(_registry);
            }
            if (MovementProvider == null)
            {
                MovementProvider = this.gameObject.AddComponent<MoveTowardsTargetProvider>();
                _movementProvider = MovementProvider as MonoBehaviour;
            }
            this.EndStart(ref _started);
        }

        protected override void InteractorAdded(SnapInteractor interactor)
        {
            base.InteractorAdded(interactor);
            if (SnapPoseDelegate != null)
            {
                SnapPoseDelegate.TrackElement(interactor.Identifier, interactor.SnapPose);
            }
        }

        protected override void InteractorRemoved(SnapInteractor interactor)
        {
            base.InteractorRemoved(interactor);
            if (SnapPoseDelegate != null)
            {
                SnapPoseDelegate.UntrackElement(interactor.Identifier);
            }
        }

        protected override void SelectingInteractorAdded(SnapInteractor interactor)
        {
            base.SelectingInteractorAdded(interactor);
            if (SnapPoseDelegate != null)
            {
                SnapPoseDelegate.SnapElement(interactor.Identifier, interactor.SnapPose);
            }
        }

        protected override void SelectingInteractorRemoved(SnapInteractor interactor)
        {
            base.SelectingInteractorRemoved(interactor);
            if (SnapPoseDelegate != null)
            {
                SnapPoseDelegate.UnsnapElement(interactor.Identifier);
            }
        }

        public void InteractorHoverUpdated(SnapInteractor interactor)
        {
            if (SnapPoseDelegate != null)
            {
                SnapPoseDelegate.MoveTrackedElement(interactor.Identifier, interactor.SnapPose);
            }
        }

        public bool PoseForInteractor(SnapInteractor interactor, out Pose result)
        {
            if (SnapPoseDelegate != null)
            {
                if (SnapPoseDelegate.SnapPoseForElement(interactor.Identifier, interactor.SnapPose, out result))
                {
                    if (result.position == transform.position)
                    {
                        result.position = FindAlternativePosition();
                    }
                    return true;
                }
            }

            result = this.transform.GetPose();
            return true;
        }

        private Vector3 FindAlternativePosition()
        {
            return transform.position + Vector3.forward;
        }

        public IMovement GenerateMovement(in Pose from, SnapInteractor interactor)
        {
            if (PoseForInteractor(interactor, out Pose to))
            {
                IMovement movement = MovementProvider.CreateMovement();
                movement.StopAndSetPose(from);
                movement.MoveTo(to);
                return movement;
            }
            return null;
        }

        #region Inject

        public void InjectAllSnapInteractable(Rigidbody rigidbody)
        {
            InjectRigidbody(rigidbody);
        }

        public void InjectRigidbody(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        public void InjectOptionalMovementProvider(IMovementProvider provider)
        {
            _movementProvider = provider as UnityEngine.Object;
            MovementProvider = provider;
        }

        public void InjectOptionalSnapPoseDelegate(ISnapPoseDelegate snapPoseDelegate)
        {
            _snapPoseDelegate = snapPoseDelegate as UnityEngine.Object;
            SnapPoseDelegate = snapPoseDelegate;
        }

        #endregion
    }
}
