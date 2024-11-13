using Oculus.Interaction.HandGrab;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
    /// <summary>
    /// SnapInteractables provide Pose targets for SnapInteractors to translate and rotate towards.
    /// </summary>
    public class SnapInteractable2 : Interactable<SnapInteractor, SnapInteractable>,
        IRigidbodyRef
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

        /// <summary>
        /// Moves the tracked element using the <cref="ISnapPoseDelegate" />.
        /// </summary>
        public void InteractorHoverUpdated(SnapInteractor interactor)
        {
            if (SnapPoseDelegate != null)
            {
                SnapPoseDelegate.MoveTrackedElement(interactor.Identifier, interactor.SnapPose);
            }
        }

        /// <summary>
        /// Sets the pose for the interactor.
        /// <param name="interactor">The SnapInteractor object.</param>
        /// <param name="result">The resulting pose.</param>
        /// </summary>
        public bool PoseForInteractor(SnapInteractor interactor, out Pose result)
        {
            if (SnapPoseDelegate != null)
            {
                if (SnapPoseDelegate.SnapPoseForElement(interactor.Identifier, interactor.SnapPose, out result))
                {
                    // Проверяем, чтобы позиция не совпала с текущей
                    if (result.position == transform.position)
                    {
                        // Если позиция совпадает с текущей, пытаемся найти другую позицию
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
            // Реализуем логику для поиска альтернативной позиции
            // Например, сдвигаем позицию на 1 юнит вперед
            return transform.position + Vector3.forward; 
        }

        /// <summary>
        /// Generates a movement that when applied will move the interactor from a start to a target pose.
        /// <param name="from">The starting position of the interactor.</param>
        /// <param name="interactor">The interactor to move.</param>
        /// </summary>
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

        /// <summary>
        /// Sets all required values for a snap interactable to a dynamically instantiated GameObject.
        /// </summary>
        public void InjectAllSnapInteractable(Rigidbody rigidbody)
        {
            InjectRigidbody(rigidbody);
        }

        /// <summary>
        /// Sets a Rigidbody on a dynamically instantiated GameObject.
        /// </summary>
        public void InjectRigidbody(Rigidbody rigidbody)
        {
            _rigidbody = rigidbody;
        }

        /// <summary>
        /// Sets a movement provider on a dynamically instantiated GameObject.
        /// </summary>
        public void InjectOptionalMovementProvider(IMovementProvider provider)
        {
            _movementProvider = provider as UnityEngine.Object;
            MovementProvider = provider;
        }

        /// <summary>
        /// Sets a snap pose delegate on a dynamically instantiated GameObject.
        /// </summary>
        public void InjectOptionalSnapPoseDelegate(ISnapPoseDelegate snapPoseDelegate)
        {
            _snapPoseDelegate = snapPoseDelegate as UnityEngine.Object;
            SnapPoseDelegate = snapPoseDelegate;
        }

        #endregion
    }
}
