/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Core logic for Sticky Stick Stuck for 2D.
*******************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

#if (UNITY_EDITOR)
using UnityEditor;
#endif

namespace StickyStickStuck
{
    [RequireComponent(typeof(Rigidbody2D)), AddComponentMenu("Physics 2D/Sticky Stick Stuck 2D", -1)]
    public class SSS2D : MonoBehaviour
    {
        #region Classes

        //Manages all filter type properties
        [System.Serializable]
        public class FilterProperties
        {
            //Filter effect types
            public enum EffectType
            {
                Effect,
                DontEffect,
            }

            //Tag filter properties
            [System.Serializable]
            public class TagFilter
            {
                [SerializeField, Tooltip("Effect type for tag filter options.")]
                private EffectType effectType = EffectType.Effect;
                public EffectType _effectType
                {
                    get { return effectType; }
                    set { effectType = value; }
                }

                [SerializeField, Tooltip("Tag name used for filter options.")]
                private string tag = string.Empty;
                public string Tag
                {
                    get { return tag; }
                    set { tag = value; }
                }
            }

            //GameObject filter properties
            [System.Serializable]
            public class GameObjectFilter
            {
                [SerializeField, Tooltip("Effect type for Gameobject filter options.")]
                private EffectType effectType = EffectType.Effect;
                public EffectType _effectType
                {
                    get { return effectType; }
                    set { effectType = value; }
                }

                [SerializeField, Tooltip("Gameobject used for filter options.")]
                private GameObject gameObject;
                public GameObject _gameObject
                {
                    get { return gameObject; }
                    set { gameObject = value; }
                }
            }

            //Collider filter properties
            [System.Serializable]
            public class BoundsFilter
            {
                [SerializeField, Tooltip("Effect type for bounds collider filter options.")]
                private EffectType effectType = EffectType.Effect;
                public EffectType _effectType
                {
                    get { return effectType; }
                    set { effectType = value; }
                }

                [SerializeField, Tooltip("Bound collider used for filter options.")]
                private Collider2D collider;
                public Collider2D _collider
                {
                    get { return collider; }
                    set { collider = value; }
                }
            }

            //Physic material filter properties
            [System.Serializable]
            public class PhysicMaterialFilter
            {
                [SerializeField, Tooltip("Effect type for physic material filter options.")]
                private EffectType effectType = EffectType.Effect;
                public EffectType _effectType
                {
                    get { return effectType; }
                    set { effectType = value; }
                }

                [SerializeField, Tooltip("Physic material used for filter options.")]
                private PhysicsMaterial2D physicMaterial;
                public PhysicsMaterial2D _physicMaterial
                {
                    get { return physicMaterial; }
                    set { physicMaterial = value; }
                }
            }

            // Filter Properties Constructor
            public FilterProperties()
            {
                gameObjectFilter = new List<GameObjectFilter>();
                tagFilter = new List<TagFilter>();
                boundsFilter = new List<BoundsFilter>();
                physicMaterialFilter = new List<PhysicMaterialFilter>();
            }

            [SerializeField, Tooltip("Used to filter out gameobjects, has authority over tags, colliders, physicMaterial, and layermasks.")]
            private List<GameObjectFilter> gameObjectFilter;
            public List<GameObjectFilter> _gameObjectFilter
            {
                get { return gameObjectFilter; }
                set { gameObjectFilter = value; }
            }

            [SerializeField, Tooltip("Used to filter out tags, has authority over colliders, physicMaterial, and layermasks.")]
            private List<TagFilter> tagFilter;
            public List<TagFilter> _tagFilter
            {
                get { return tagFilter; }
                set { tagFilter = value; }
            }

            [SerializeField, Tooltip("Used to filter out collider bounds, has authority over physicMaterial, and layermasks.")]
            public List<BoundsFilter> boundsFilter;
            public List<BoundsFilter> _boundsFilter
            {
                get { return boundsFilter; }
                set { boundsFilter = value; }
            }

            [SerializeField, Tooltip("Used to filter out physic material, has authority over layermasks.")]
            public List<PhysicMaterialFilter> physicMaterialFilter;
            public List<PhysicMaterialFilter> _physicMaterialFilter
            {
                get { return physicMaterialFilter; }
                set { physicMaterialFilter = value; }
            }

            [SerializeField, Tooltip("Used for fildering LayerMasks.")]
            private LayerMask layerMaskFilter = -1;
            public LayerMask _layerMaskFilter
            {
                get { return layerMaskFilter; }
                set { layerMaskFilter = value; }
            }

            //ValidateFilters all filter options
            public bool ValidateFilters(GameObject gameobjectFilter, Collider2D coll)
            {
                bool value = true;

                if (_gameObjectFilter.Count > 0)
                {
                    for (int i = 0; i < _gameObjectFilter.Count; i++)
                    {
                        switch (_gameObjectFilter[i]._effectType)
                        {
                            case EffectType.Effect:
                                if (_gameObjectFilter[i]._gameObject == gameobjectFilter)
                                {
                                    return true;
                                }
                                break;
                            case EffectType.DontEffect:
                                if (_gameObjectFilter[i]._gameObject == gameobjectFilter)
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }

                if (_tagFilter.Count > 0)
                {
                    for (int i = 0; i < _tagFilter.Count; i++)
                    {
                        switch (_tagFilter[i]._effectType)
                        {
                            case EffectType.Effect:
                                if (gameobjectFilter.transform.gameObject.CompareTag(_tagFilter[i].Tag))
                                {
                                    return true;
                                }
                                break;
                            case EffectType.DontEffect:
                                if (gameobjectFilter.transform.gameObject.CompareTag(_tagFilter[i].Tag))
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }

                if (_boundsFilter.Count > 0)
                {
                    for (int i = 0; i < _boundsFilter.Count; i++)
                    {
                        switch (_boundsFilter[i]._effectType)
                        {
                            case EffectType.Effect:
                                if (_boundsFilter[i]._collider.bounds.Contains(gameobjectFilter.transform.position))
                                {
                                    return true;
                                }
                                break;
                            case EffectType.DontEffect:
                                if (_boundsFilter[i]._collider.bounds.Contains(gameobjectFilter.transform.position))
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }

                if (_physicMaterialFilter.Count > 0)
                {
                    for (int i = 0; i < _physicMaterialFilter.Count; i++)
                    {
                        switch (_physicMaterialFilter[i]._effectType)
                        {
                            case EffectType.Effect:
                                if (_physicMaterialFilter[i]._physicMaterial == coll.sharedMaterial)
                                {
                                    return true;
                                }
                                break;
                            case EffectType.DontEffect:
                                if (_physicMaterialFilter[i]._physicMaterial == coll.sharedMaterial)
                                {
                                    return false;
                                }
                                break;
                        }
                    }
                }

                if (((1 << gameobjectFilter.layer) & _layerMaskFilter) != 0)
                {
                    value = true;
                }
                else if (((1 << gameobjectFilter.layer) & _layerMaskFilter) == 0)
                {
                    value = false;
                }

                return value;
            }
        }

        [System.Serializable]
        public class FixedJointProperties
        {
            [SerializeField, Tooltip("Check this box to enable the ability for the two connected objects to collide with each other.")]
            private bool enableCollision = false;
            public bool EnableCollision
            {
                get { return enableCollision; }
                set { enableCollision = value; }
            }

            [SerializeField, Tooltip("The degree to which you want to suppress spring oscillation: In the range 0 to 1, the higher the value, the less movement.")]
            private float dampingRatio = 0f;
            public float DampingRatio
            {
                get { return dampingRatio; }
                set { dampingRatio = value; }
            }

            [SerializeField, Tooltip("The frequency at which the spring oscillates while the objects are approaching the separation distance you want (measured in cycles per second): In the range 1 to 1,000,000, the higher the value, the stiffer the spring. Note that if the frequency is set to 0, the spring will be completely stiff.")]
            private float frequency = 0f;
            public float Frequency
            {
                get { return frequency; }
                set { frequency = value; }
            }

            [SerializeField, Tooltip("Specify the force level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakForce = Mathf.Infinity;
            public float BreakForce
            {
                get { return breakForce; }
                set { breakForce = value; }
            }

            [SerializeField, Tooltip("Specify the torque level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakTorque = Mathf.Infinity;
            public float BreakTorque
            {
                get { return breakTorque; }
                set { breakTorque = value; }
            }
        }

        [System.Serializable]
        public class SpringJointProperties
        {
            [SerializeField, Tooltip("Can the two connected objects collide with each other? Check the box for yes.")]
            private bool enableCollision = false;
            public bool EnableCollision
            {
                get { return enableCollision; }
                set { enableCollision = value; }
            }

            [SerializeField, Tooltip("The distance that the spring should attempt to maintain between the two objects. (Can be set manually.)")]
            private float distance = 0.005f;
            public float Distance
            {
                get { return distance; }
                set { distance = value; }
            }

            [SerializeField, Tooltip("The degree to which you want to suppress spring oscillation: In the range 0 to 1, the higher the value, the less movement.")]
            private float dampingRatio = 0f;
            public float DampingRatio
            {
                get { return dampingRatio; }
                set { dampingRatio = value; }
            }

            [SerializeField, Tooltip("The frequency at which the spring oscillates while the objects are approaching the separation distance you want (measured in cycles per second): In the range 0 to 1,000,000 - the higher the value, the stiffer the spring.")]
            private float frequency = 1f;
            public float Frequency
            {
                get { return frequency; }
                set { frequency = value; }
            }

            [SerializeField, Tooltip("Specify the force level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakForce = Mathf.Infinity;
            public float BreakForce
            {
                get { return breakForce; }
                set { breakForce = value; }
            }

            [SerializeField, Tooltip("Specify the torque level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakTorque = Mathf.Infinity;
            public float BreakTorque
            {
                get { return breakTorque; }
                set { breakTorque = value; }
            }
        }

        [System.Serializable]
        public class DistanceJointProperties
        {
            [SerializeField, Tooltip("Can the two connected objects collide with each other? Check the box for yes.")]
            private bool enableCollision = false;
            public bool EnableCollision
            {
                get { return enableCollision; }
                set { enableCollision = value; }
            }

            [SerializeField, Tooltip("Specify the distance that the Distance Joint 2D keeps between the two GameObjects.")]
            private float distance = 0.005f;
            public float Distance
            {
                get { return distance; }
                set { distance = value; }
            }

            [SerializeField, Tooltip("If enabled, the Distance Joint 2D only enforces a maximum distance, so the connected GameObjects can still move closer to each other, but not further than the Distance field defines. If this is not enabled, the distance between the GameObjects is fixed.")]
            private bool maxDistanceOnly = false;
            public bool MaxDistanceOnly
            {
                get { return maxDistanceOnly; }
                set { maxDistanceOnly = value; }
            }

            [SerializeField, Tooltip("Specify the force level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakForce = Mathf.Infinity;
            public float BreakForce
            {
                get { return breakForce; }
                set { breakForce = value; }
            }

            [SerializeField, Tooltip("Specify the torque level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakTorque = Mathf.Infinity;
            public float BreakTorque
            {
                get { return breakTorque; }
                set { breakTorque = value; }
            }
        }

        [System.Serializable]
        public class FrictionJointProperties
        {
            [SerializeField, Tooltip("Can the two connected objects collide with each other? Check the box for yes.")]
            private bool enableCollision = false;
            public bool EnableCollision
            {
                get { return enableCollision; }
                set { enableCollision = value; }
            }

            [SerializeField, Tooltip("Sets the linear (or straight line) movement between joined GameObjects. A high value resists the linear movement between GameObjects.")]
            private float maxForce = 1f;
            public float MaxForce
            {
                get { return maxForce; }
                set { maxForce = value; }
            }

            [SerializeField, Tooltip("Sets the angular (or rotation) movement between joined GameObjects. A high value resists the rotation movement between GameObjects.")]
            private float maxTorque = 1f;
            public float MaxTorque
            {
                get { return maxTorque; }
                set { maxTorque = value; }
            }

            [SerializeField, Tooltip("Specify the force level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakForce = Mathf.Infinity;
            public float BreakForce
            {
                get { return breakForce; }
                set { breakForce = value; }
            }

            [SerializeField, Tooltip("Specify the torque level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakTorque = Mathf.Infinity;
            public float BreakTorque
            {
                get { return breakTorque; }
                set { breakTorque = value; }
            }
        }

        [System.Serializable]
        public class HingeJointProperties
        {
            [System.Serializable]
            public class JointMotor
            {
                [SerializeField, Tooltip("Set the target motor speed, in degrees per second.")]
                private float motorSpeed;
                public float _motorSpeed
                {
                    get { return motorSpeed; }
                    set { motorSpeed = value; }
                }

                [SerializeField, Tooltip("Set the maximum torque (or rotation) the motor can apply while attempting to reach the target speed.")]
                private float maximumMotorForce = 10000f;
                public float _maximumMotorForce
                {
                    get { return maximumMotorForce; }
                    set { maximumMotorForce = value; }
                }
            }

            [System.Serializable]
            public class JointAngleLimits
            {
                [SerializeField, Tooltip("Upper angular limit of rotation.")]
                private float lowerAngle;
                public float _lowerAngle
                {
                    get { return lowerAngle; }
                    set { lowerAngle = value; }
                }

                [SerializeField, Tooltip("Lower angular limit of rotation.")]
                private float upperAngle = 359f;
                public float _upperAngle
                {
                    get { return upperAngle; }
                    set { upperAngle = value; }
                }
            }

            [SerializeField, Tooltip("Can the two connected objects collide with each other? Check the box for yes.")]
            private bool enableCollision = false;
            public bool EnableCollision
            {
                get { return enableCollision; }
                set { enableCollision = value; }
            }

            [SerializeField, Tooltip("Check the box to enable the hinge motor.")]
            private bool useMotor;
            public bool _useMotor
            {
                get { return useMotor; }
                set { useMotor = value; }
            }

            [SerializeField, Tooltip("Use this to change Motor settings.")]
            private JointMotor motor;
            public JointMotor _motor
            {
                get { return motor; }
                set { motor = value; }
            }

            [SerializeField, Tooltip("Check this box to limit the rotation angle")]
            private bool useLimits;
            public bool _useLimits
            {
                get { return useLimits; }
                set { useLimits = value; }
            }

            [SerializeField, Tooltip("Use these settings to set limits if Use Limits is enabled.")]
            private JointAngleLimits angleLimits;
            public JointAngleLimits _angleLimits
            {
                get { return angleLimits; }
                set { angleLimits = value; }
            }

            [SerializeField, Tooltip("Specify the force level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakForce = Mathf.Infinity;
            public float BreakForce
            {
                get { return breakForce; }
                set { breakForce = value; }
            }

            [SerializeField, Tooltip("Specify the torque level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakTorque = Mathf.Infinity;
            public float BreakTorque
            {
                get { return breakTorque; }
                set { breakTorque = value; }
            }
        }

        [System.Serializable]
        public class RelativeJointProperties
        {
            [SerializeField, Tooltip("Can the two connected objects collide with each other? Check the box for yes.")]
            private bool enableCollision = false;
            public bool EnableCollision
            {
                get { return enableCollision; }
                set { enableCollision = value; }
            }

            [SerializeField, Tooltip("Sets the linear (straight line) offset between joined objects - a high value (of up to 1,000) uses high force to maintain the offset.")]
            private float maxForce = 10000f;
            public float MaxForce
            {
                get { return maxForce; }
                set { maxForce = value; }
            }

            [SerializeField, Tooltip("Sets the angular (rotation) movement between joined objects - a high value (of up to 1,000) uses high force to maintain the offset.")]
            private float maxTorque = 10000f;
            public float MaxTorque
            {
                get { return maxTorque; }
                set { maxTorque = value; }
            }

            [SerializeField, Tooltip("Tweaks the joint to make sure it behaves as required. (Increasing the Max Force or Max Torque may affect behaviour so that the joint doesn’t reach its target, use this setting to correct it.) The default setting of 0.3 is usually appropriate but it may need tweaking between the range of 0 and 1.")]
            private float correctionScale = .3f;
            public float CorrectionScale
            {
                get { return correctionScale; }
                set { correctionScale = value; }
            }

            [SerializeField, Tooltip("Check this box to automatically set and maintain the distance and angle between the connected objects. (Selecting this option means you don’t need to manually enter the Linear Offset and Angular Offset.)")]
            private bool autoConfigureOffset = true;
            public bool AutoConfigureOffset
            {
                get { return autoConfigureOffset; }
                set { autoConfigureOffset = value; }
            }

            [SerializeField, Tooltip("Enter local space co-ordinates to specify and maintain the distance between the connected objects.")]
            private Vector2 linearOffset;
            public Vector2 LinearOffset
            {
                get { return linearOffset; }
                set { linearOffset = value; }
            }

            [SerializeField, Tooltip("Enter local space co-ordinates to specify and maintain the angle between the connected objects.")]
            private float angularOffset = 0f;
            public float AngularOffset
            {
                get { return angularOffset; }
                set { angularOffset = value; }
            }

            [SerializeField, Tooltip("Specify the force level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakForce = Mathf.Infinity;
            public float BreakForce
            {
                get { return breakForce; }
                set { breakForce = value; }
            }

            [SerializeField, Tooltip("Specify the torque level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakTorque = Mathf.Infinity;
            public float BreakTorque
            {
                get { return breakTorque; }
                set { breakTorque = value; }
            }
        }

        [System.Serializable]
        public class SliderJointProperties
        {
            [System.Serializable]
            public class JointTranslationLimits
            {
                [SerializeField, Tooltip("The minimum distance the object can be from the connected anchor point.")]
                private float lowerTranslation;
                public float _lowerTranslation
                {
                    get { return lowerTranslation; }
                    set { lowerTranslation = value; }
                }

                [SerializeField, Tooltip("The maximum distance the object can be from the connected anchor point.")]
                private float upperTranslation;
                public float _upperTranslation
                {
                    get { return upperTranslation; }
                    set { upperTranslation = value; }
                }
            }

            [SerializeField, Tooltip("Can the two connected objects collide with each other? Check the box for yes.")]
            private bool enableCollision = false;
            public bool EnableCollision
            {
                get { return enableCollision; }
                set { enableCollision = value; }
            }

            [SerializeField, Tooltip("Enter the the angle that the joint keeps between the two objects.")]
            private float angle;
            public float Angle
            {
                get { return angle; }
                set { angle = value; }
            }

            [SerializeField, Tooltip("Use the sliding motor? Check the box for yes.")]
            private bool useMotor;
            public bool _useMotor
            {
                get { return useMotor; }
                set { useMotor = value; }
            }

            [SerializeField, Tooltip("")]
            private HingeJointProperties.JointMotor motor;
            public HingeJointProperties.JointMotor _motor
            {
                get { return motor; }
                set { motor = value; }
            }

            [SerializeField, Tooltip("Should there be limits to the linear (straight line) force? Check the box for yes.")]
            private bool useLimits;
            public bool _useLimits
            {
                get { return useLimits; }
                set { useLimits = value; }
            }

            [SerializeField, Tooltip("")]
            private SliderJointProperties.JointTranslationLimits translateLimits;
            public SliderJointProperties.JointTranslationLimits _translateLimits
            {
                get { return translateLimits; }
                set { translateLimits = value; }
            }

            [SerializeField, Tooltip("Specify the force level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakForce = Mathf.Infinity;
            public float BreakForce
            {
                get { return breakForce; }
                set { breakForce = value; }
            }

            [SerializeField, Tooltip("Specify the torque level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakTorque = Mathf.Infinity;
            public float BreakTorque
            {
                get { return breakTorque; }
                set { breakTorque = value; }
            }
        }

        [System.Serializable]
        public class TargetJointProperties
        {
            [SerializeField, Tooltip("Can the two connected objects collide with each other? Check the box for yes.")]
            private bool enableCollision = false;
            public bool EnableCollision
            {
                get { return enableCollision; }
                set { enableCollision = value; }
            }

            [SerializeField, Tooltip("Sets the force that the joint can apply when attempting to move the object to the target position. The higher the value, the higher the maximum force.")]
            private float maxForce = 10000;
            public float _maxForce
            {
                get { return maxForce; }
                set { maxForce = value; }
            }

            [SerializeField, Tooltip("The degree to which you want to suppress spring oscillation: In the range 0 to 1, the higher the value, the less movement.")]
            private int dampingRatio = 1;
            public int _dampingRatio
            {
                get { return dampingRatio; }
                set { dampingRatio = value; }
            }

            [SerializeField, Tooltip("The frequency at which the spring oscillates while the objects are approaching the separation distance you want (measured in cycles per second): In the range 0 to 1,000,000 -, the higher the value, the stiffer the spring.")]
            private float frequency = 5f;
            public float _frequency
            {
                get { return frequency; }
                set { frequency = value; }
            }

            [SerializeField, Tooltip("Specify the force level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakForce = Mathf.Infinity;
            public float BreakForce
            {
                get { return breakForce; }
                set { breakForce = value; }
            }

            [SerializeField, Tooltip("Specify the torque level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakTorque = Mathf.Infinity;
            public float BreakTorque
            {
                get { return breakTorque; }
                set { breakTorque = value; }
            }
        }

        [System.Serializable]
        public class WheelJointProperties
        {
            [System.Serializable]
            public class Suspension
            {
                [SerializeField, Tooltip("The degree to which you want to suppress spring oscillation in the suspension: In the range 0 to 1, the higher the value, the less movement.")]
                private float dampingRatio = .7f;
                public float _dampingRatio
                {
                    get { return dampingRatio; }
                    set { dampingRatio = value; }
                }

                [SerializeField, Tooltip("The frequency at which the spring in the suspension oscillates while the objects are approaching the separation distance you want (measured in cycles per second): In the range 0 to 1,000,000 - the higher the value, the stiffer the suspension spring.")]
                private float frequency = 2f;
                public float _frequency
                {
                    get { return frequency; }
                    set { frequency = value; }
                }

                [SerializeField, Tooltip("The world movement angle for the suspension.")]
                private float angle = 90f;
                public float _angle
                {
                    get { return angle; }
                    set { angle = value; }
                }
            }


            [SerializeField, Tooltip("Can the two connected objects collide with each other? Check the box for yes.")]
            private bool enableCollision = false;
            public bool EnableCollision
            {
                get { return enableCollision; }
                set { enableCollision = value; }
            }

            [SerializeField, Tooltip("")]
            private Suspension suspensio;
            public Suspension _suspensio
            {
                get { return suspensio; }
                set { suspensio = value; }
            }

            [SerializeField, Tooltip("Apply a motor force to the wheel? Check the box for yes.")]
            private bool useMotor;
            public bool _useMotor
            {
                get { return useMotor; }
                set { useMotor = value; }
            }

            [SerializeField, Tooltip("")]
            private HingeJointProperties.JointMotor motor;
            public HingeJointProperties.JointMotor _motor
            {
                get { return motor; }
                set { motor = value; }
            }

            [SerializeField, Tooltip("Specify the force level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakForce = Mathf.Infinity;
            public float BreakForce
            {
                get { return breakForce; }
                set { breakForce = value; }
            }

            [SerializeField, Tooltip("Specify the torque level needed to break and so delete the joint. Infinity means it is unbreakable.")]
            private float breakTorque = Mathf.Infinity;
            public float BreakTorque
            {
                get { return breakTorque; }
                set { breakTorque = value; }
            }
        }

        [System.Serializable]
        public class ConnectedObjectData
        {
            [SerializeField, Tooltip("GameObject parent transform.")]
            private Transform parent;
            public Transform Parent
            {
                get { return parent; }
                set { parent = value; }
            }

            [SerializeField, Tooltip("Connected GameObject.")]
            private GameObject gameObject;
            public GameObject _gameObject
            {
                get { return gameObject; }
                set { gameObject = value; }
            }

            [SerializeField, Tooltip("Connected GameObject colliders.")]
            private Collider2D[] colliders;
            public Collider2D[] _colliders
            {
                get { return colliders; }
                set { colliders = value; }
            }

            [SerializeField, Tooltip("Connected GameObject FixedJoint.")]
            private FixedJoint2D fixedJoint;
            public FixedJoint2D _fixedJoint
            {
                get { return fixedJoint; }
                set { fixedJoint = value; }
            }

            [SerializeField, Tooltip("Connected GameObject SpringJoint.")]
            private SpringJoint2D springJoint;
            public SpringJoint2D _springJoint
            {
                get { return springJoint; }
                set { springJoint = value; }
            }

            [SerializeField, Tooltip("Connected GameObject DistanceJoint.")]
            private DistanceJoint2D distanceJoint;
            public DistanceJoint2D _distanceJoint
            {
                get { return distanceJoint; }
                set { distanceJoint = value; }
            }

            [SerializeField, Tooltip("Connected GameObject FrictionJoint.")]
            private FrictionJoint2D frictionJoint;
            public FrictionJoint2D _frictionJoint
            {
                get { return frictionJoint; }
                set { frictionJoint = value; }
            }

            [SerializeField, Tooltip("Connected GameObject HingeJoint.")]
            private HingeJoint2D hingeJoint;
            public HingeJoint2D _hingeJoint
            {
                get { return hingeJoint; }
                set { hingeJoint = value; }
            }

            [SerializeField, Tooltip("Connected GameObject RelativeJoint.")]
            private RelativeJoint2D relativeJoint;
            public RelativeJoint2D _relativeJoint
            {
                get { return relativeJoint; }
                set { relativeJoint = value; }
            }

            [SerializeField, Tooltip("Connected GameObject SliderJoint.")]
            private SliderJoint2D sliderJoint;
            public SliderJoint2D _sliderJoint
            {
                get { return sliderJoint; }
                set { sliderJoint = value; }
            }

            [SerializeField, Tooltip("Connected GameObject TargetJoint.")]
            private TargetJoint2D targetJoint;
            public TargetJoint2D _targetJoint
            {
                get { return targetJoint; }
                set { targetJoint = value; }
            }

            [SerializeField, Tooltip("Connected GameObject WheelJoint.")]
            private WheelJoint2D wheelJoint;
            public WheelJoint2D _wheelJoint
            {
                get { return wheelJoint; }
                set { wheelJoint = value; }
            }
        }

        #endregion

        #region enums

        public enum JointType
        {
            None = 0,
            Fixed = 1,
            Spring = 2,
            Distance = 3,
            Friction = 4,
            Hinge = 5,
            Relative = 6,
            Slider = 7,
            Target = 8,
            Wheel = 9,
        }

        #endregion

        #region Properties

        static public string WarningMessageClosestPoint_5_6 = "Requires Upgrading Project to Unity 2019.4 or Higher.";
        
        //Events
        static public event Action<SSS2D> OnStick2D;
        static public event Action<SSS2D> OnUnStick2D;
        static public event Action<SSS2D> OnStickBreak2D;

        [SerializeField, Tooltip("Enable/Disables Sticky Stick Stuck, if disabled will cleanup all join mess.")]
        private bool enable = true;
        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }
        [SerializeField, Tooltip("Pauses the Sticky Stick Stuck.")]
        private bool paused = false;
        public bool Paused
        {
            get { return paused; }
            set { paused = value; }
        }
        [SerializeField, Tooltip("How the objects should parent when stuck together.")]
        private SSS.ParentType parentType;
        public SSS.ParentType _parentType
        {
            get { return parentType; }
            set { parentType = value; }
        }
        [SerializeField, Tooltip("The joint type when stuck together.")]
        private JointType jointType = JointType.Fixed;
        public JointType _jointType
        {
            get { return jointType; }
            set { jointType = value; }
        }
        [SerializeField, Tooltip("Fixed Joints restricts an object’s movement to be dependent upon another object.")]
        private FixedJointProperties fixedJointProperties;
        public FixedJointProperties _fixedJointProperties
        {
            get { return fixedJointProperties; }
            set { fixedJointProperties = value; }
        }
        [SerializeField, Tooltip("The Spring Joint joins two Rigidbodies together but allows the distance between them to change as though they were connected by a spring.")]
        private SpringJointProperties springJointProperties;
        public SpringJointProperties _springJointProperties
        {
            get { return springJointProperties; }
            set { springJointProperties = value; }
        }
        [SerializeField, Tooltip("")]
        private DistanceJointProperties distanceJointProperties;
        public DistanceJointProperties _distanceJointProperties
        {
            get { return distanceJointProperties; }
            set { distanceJointProperties = value; }
        }
        [SerializeField, Tooltip("")]
        private FrictionJointProperties frictionJointProperties;
        public FrictionJointProperties _frictionJointProperties
        {
            get { return frictionJointProperties; }
            set { frictionJointProperties = value; }
        }
        [SerializeField, Tooltip("")]
        private HingeJointProperties hingeJointProperties;
        public HingeJointProperties _hingeJointProperties
        {
            get { return hingeJointProperties; }
            set { hingeJointProperties = value; }
        }
        [SerializeField, Tooltip("")]
        private RelativeJointProperties relativeJointProperties;
        public RelativeJointProperties _relativeJointProperties
        {
            get { return relativeJointProperties; }
            set { relativeJointProperties = value; }
        }
        [SerializeField, Tooltip("")]
        private SliderJointProperties sliderJointProperties;
        public SliderJointProperties _sliderJointProperties
        {
            get { return sliderJointProperties; }
            set { sliderJointProperties = value; }
        }
        [SerializeField, Tooltip("")]
        private TargetJointProperties targetJointProperties;
        public TargetJointProperties _targetJointProperties
        {
            get { return targetJointProperties; }
            set { targetJointProperties = value; }
        }
        [SerializeField, Tooltip("")]
        private WheelJointProperties wheelJointProperties;
        public WheelJointProperties _wheelJointProperties
        {
            get { return wheelJointProperties; }
            set { wheelJointProperties = value; }
        }
        [SerializeField, Tooltip("Contains all UnityEvent for StickyStickStuck.")]
        private SSS.EventProperties eventProperties;
        public SSS.EventProperties _eventProperties
        {
            get { return eventProperties; }
            set { eventProperties = value; }
        }
        [SerializeField, Tooltip("Filter stick options.")]
        private FilterProperties filterProperties;
        public FilterProperties _filterProperties
        {
            get { return filterProperties; }
            set { filterProperties = value; }
        }
        [SerializeField, Tooltip("Sticks to trigger colliders.")]
        private bool stickToTrigger = false;
        public bool StickToTrigger
        {
            get { return stickToTrigger; }
            set { stickToTrigger = value; }
        }
        [SerializeField, Tooltip("Sticks when colliding in trigger area.")]
        private bool stickInTriggerArea = false;
        public bool StickInTriggerArea
        {
            get { return stickInTriggerArea; }
            set { stickInTriggerArea = value; }
        }
        [SerializeField, Tooltip("Sticks to GameObjects that dont have a rigidbody.")]
        private bool stickToNonRigidbody;
        public bool StickToNonRigidbody
        {
            get { return stickToNonRigidbody; }
            set { stickToNonRigidbody = value; }
        }
        [SerializeField, Tooltip("Sticks to GameObjects that have the StickyStickStuck component.")]
        private bool stickOnSticky = false;
        public bool StickOnSticky
        {
            get { return stickOnSticky; }
            set { stickOnSticky = value; }
        }
        [SerializeField, Tooltip("When sticks, this sets the GameObjects transform offset.")]
        private Vector3 stickOffset = Vector3.zero;
        public Vector3 StickOffset
        {
            get { return stickOffset; }
            set { stickOffset = value; }
        }
        [SerializeField, Tooltip("The target impact velocity in order for the object to stick.")]
        private float stickImpactVelocity;
        public float StickImpactVelocity
        {
            get { return stickImpactVelocity; }
            set { stickImpactVelocity = value; }
        }
        [SerializeField, Tooltip("Ignores all colliders that it sticks too.")]
        private bool ignoreColliders = false;
        public bool IgnoreColliders
        {
            get { return ignoreColliders; }
            set { ignoreColliders = value; }
        }
        [SerializeField, Tooltip("Recursively adds the StickyStickStuck component to whatever it touchs.")]
        private bool stickRecursively = false;
        public bool StickRecursively
        {
            get { return stickRecursively; }
            set { stickRecursively = value; }
        }
        [SerializeField, Tooltip("Max amount of objects that can be effected.")]
        private int maxEffectedItems = 1;
        public int MaxEffectedItems
        {
            get { return maxEffectedItems; }
            set { maxEffectedItems = value; }
        }
        [SerializeField, Tooltip("Contains all debug options for StickyStickStuck.")]
        private SSS.DebugProperties debugProperties;
        public SSS.DebugProperties _debugProperties
        {
            get { return debugProperties; }
            set { debugProperties = value; }
        }

        //Used for telling which StickYStickStuck object is the parent
        [SerializeField, Tooltip("Flag for if the StickyStickStuck is the infected parent.")]
        private bool isInfectParent = true;
        public bool IsInfectParent
        {
            get { return isInfectParent; }
            set { isInfectParent = value; }
        }

        [SerializeField, Tooltip("Collected StickyStickStuck data.")]
        private List<ConnectedObjectData> connectedObjectDataLists;
        public List<ConnectedObjectData> ConnectedObjectDataLists
        {
            get { return connectedObjectDataLists; }
            set { connectedObjectDataLists = value; }
        }

        private Transform startParent;
        private Rigidbody2D mainRigidbody;
        private Collider2D[] colliders;
        private float velocity = 0f;
        private bool inTriggerArea = false;

        public SSS2D()
        {
            _fixedJointProperties = new FixedJointProperties();
            _springJointProperties = new SpringJointProperties();
            _distanceJointProperties = new DistanceJointProperties();
            _frictionJointProperties = new FrictionJointProperties();
            _hingeJointProperties = new HingeJointProperties();
            _hingeJointProperties._motor = new HingeJointProperties.JointMotor();
            _hingeJointProperties._angleLimits = new HingeJointProperties.JointAngleLimits();
            _relativeJointProperties = new RelativeJointProperties();
            _sliderJointProperties = new SliderJointProperties();
            _sliderJointProperties._motor = new HingeJointProperties.JointMotor();
            _sliderJointProperties._translateLimits = new SliderJointProperties.JointTranslationLimits();
            _targetJointProperties = new TargetJointProperties();
            _wheelJointProperties = new WheelJointProperties();
            _wheelJointProperties._suspensio = new WheelJointProperties.Suspension();
            _wheelJointProperties._motor = new HingeJointProperties.JointMotor();

            _eventProperties = new SSS.EventProperties();
            _filterProperties = new FilterProperties();
            _debugProperties = new SSS.DebugProperties();

            ConnectedObjectDataLists = new List<ConnectedObjectData>();
        }

        public void CopyStickyStickStuck(SSS2D copy)
        {
            copy.Enable = this.Enable;
            copy.Paused = this.Paused;
            copy._parentType = this._parentType;
            copy._jointType = this._jointType;

            copy._fixedJointProperties = this._fixedJointProperties;
            copy._springJointProperties = this._springJointProperties;
            copy._fixedJointProperties = this._fixedJointProperties;
            copy._springJointProperties = this._springJointProperties;
            copy._distanceJointProperties = this._distanceJointProperties;
            copy._frictionJointProperties = this._frictionJointProperties;
            copy._hingeJointProperties = this._hingeJointProperties;
            copy._relativeJointProperties = this._relativeJointProperties;
            copy._sliderJointProperties = this._sliderJointProperties;
            copy._targetJointProperties = this._targetJointProperties;
            copy._wheelJointProperties = this._wheelJointProperties;

            copy.stickToTrigger = this.stickToTrigger;
            copy.StickInTriggerArea = this.StickInTriggerArea;
            copy.StickToNonRigidbody = this.StickToNonRigidbody;
            copy.StickOnSticky = this.StickOnSticky;
            copy.StickOffset = this.StickOffset;
            copy.StickImpactVelocity = this.StickImpactVelocity;
            copy.IgnoreColliders = this.IgnoreColliders;
            copy.StickRecursively = this.StickRecursively;
            copy.MaxEffectedItems = this.MaxEffectedItems;

            copy._eventProperties = this._eventProperties;
            copy._filterProperties = this._filterProperties;
            copy._debugProperties = this._debugProperties;
        }

        #endregion

        #region Unity Functions

#if (UNITY_EDITOR)

        //Creates Gizmos Icons for the StickyStickStuck
        void OnDrawGizmos()
        {
            string icon = "StickyStickStuck Icons/";
            icon = SetupIcons(icon);

            Gizmos.color = Color.white;

            if (ConnectedObjectDataLists != null)
            {
                foreach (var item in ConnectedObjectDataLists)
                {
                    Vector2 anchor = Vector3.zero;
                    Vector2 connectedAnchor = Vector3.zero;
                    if (item._fixedJoint != null)
                    {
                        anchor = this.transform.localToWorldMatrix.MultiplyPoint(item._fixedJoint.anchor);
                        connectedAnchor = item._gameObject.transform.localToWorldMatrix.MultiplyPoint(item._fixedJoint.connectedAnchor);
                    }
                    if (item._springJoint != null)
                    {
                        anchor = this.transform.localToWorldMatrix.MultiplyPoint(item._springJoint.anchor);
                        connectedAnchor = item._gameObject.transform.localToWorldMatrix.MultiplyPoint(item._springJoint.connectedAnchor);
                    }
                    if (item._distanceJoint != null)
                    {
                        anchor = this.transform.localToWorldMatrix.MultiplyPoint(item._distanceJoint.anchor);
                        connectedAnchor = item._gameObject.transform.localToWorldMatrix.MultiplyPoint(item._distanceJoint.connectedAnchor);
                    }
                    if (item._frictionJoint != null)
                    {
                        anchor = this.transform.localToWorldMatrix.MultiplyPoint(item._frictionJoint.anchor);
                        connectedAnchor = item._gameObject.transform.localToWorldMatrix.MultiplyPoint(item._frictionJoint.connectedAnchor);
                    }
                    if (item._hingeJoint != null)
                    {
                        anchor = this.transform.localToWorldMatrix.MultiplyPoint(item._hingeJoint.anchor);
                        connectedAnchor = item._gameObject.transform.localToWorldMatrix.MultiplyPoint(item._hingeJoint.connectedAnchor);
                    }
                    if (item._relativeJoint != null)
                    {
                        //anchor = this.transform.localToWorldMatrix.MultiplyPoint(item._relativeJoint.anchor);
                        //connectedAnchor = item._gameObject.transform.localToWorldMatrix.MultiplyPoint(item._relativeJoint.connectedAnchor);
                    }
                    if (item._sliderJoint != null)
                    {
                        anchor = this.transform.localToWorldMatrix.MultiplyPoint(item._sliderJoint.anchor);
                        connectedAnchor = item._gameObject.transform.localToWorldMatrix.MultiplyPoint(item._sliderJoint.connectedAnchor);
                    }
                    if (item._targetJoint != null)
                    {
                        anchor = this.transform.localToWorldMatrix.MultiplyPoint(item._targetJoint.anchor);
                        //connectedAnchor = item._gameObject.transform.localToWorldMatrix.MultiplyPoint(item._targetJoint.connectedAnchor);
                    }
                    if (item._wheelJoint != null)
                    {
                        anchor = this.transform.localToWorldMatrix.MultiplyPoint(item._wheelJoint.anchor);
                        connectedAnchor = item._gameObject.transform.localToWorldMatrix.MultiplyPoint(item._wheelJoint.connectedAnchor);
                    }

                    Gizmos.DrawIcon(anchor, icon, true);

                    if (Vector2.Distance(anchor, connectedAnchor) > .1f && item._gameObject.GetComponent<Rigidbody2D>() != null)
                    {
                        Gizmos.DrawLine(anchor, connectedAnchor);
                        Gizmos.DrawIcon(connectedAnchor, icon, true);
                    }
                }
            }
        }

        string SetupIcons(string icon)
        {
            string cgfDir = string.Format("{0}/ResurgamStudios/StickyStickStuck Package/Gizmos/StickyStickStuck Icons/", Application.dataPath);
            string dir = string.Format("{0}/Gizmos/StickyStickStuck Icons/", Application.dataPath);

            if (!Directory.Exists(dir))
            {
                if (Directory.Exists(cgfDir))
                {
                    CopyIcons(cgfDir, dir);

                    AssetDatabase.Refresh();
                }
            }

            icon = icon + "sss_icon";
            icon = icon + "_sss";
            icon = icon + ".png";

            return icon;
        }

        //Copys all cgf icons
        void CopyIcons(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir).Where(s => s.EndsWith(".png")))
            {
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
            }
        }
#endif

        void OnEnable()
        {
            CleanUp();
        }

        void Awake()
        {
            if (this.gameObject.GetComponent<Rigidbody2D>() != null)
                mainRigidbody = this.GetComponent<Rigidbody2D>();

            colliders = this.GetComponentsInChildren<Collider2D>();

            if (IsInfectParent)
                startParent = this.transform;
        }

        void FixedUpdate()
        {
            //Cleans up the mess when enabled equals false
            if (!Enable)
            {
                CleanUp();
            }

            //Checks to see if any of the stuck objects are disabled, and if so cleanup
            if (CheckForNonEnabledObjects())
            {
                CleanUp();
            }

            if (debugProperties.OutputVelocity)
                Debug.Log(string.Format("{0}: {1}", this.name, mainRigidbody.velocity.magnitude));
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            if (StickInTriggerArea)
            {
                inTriggerArea = true;
            }
            else
            {
                //Used to add the joint when collision happens
                if (Enable && !Paused && StickToTrigger)
                {
                    if (collider.attachedRigidbody != null)
                        velocity = collider.attachedRigidbody.velocity.magnitude;

                    AddStickyJoint(collider);

                    if (debugProperties.OutputHitVelocity)
                        Debug.Log(string.Format("'{0}' Hit Velocity: {1}", this.name, velocity));
                }
            }
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            if (StickInTriggerArea)
            {
                inTriggerArea = true;
            }
            else
            {
                //Used to add the joint when collision happens
                if (Enable && !Paused && StickToTrigger)
                {
                    AddStickyJoint(collider);
                }
            }
        }

        void OnTriggerExit2D(Collider2D other)
        {
            if (StickInTriggerArea)
            {
                inTriggerArea = false;
            }
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            //Used to add the joint when collision happens
            if (Enable && !Paused)
            {
                velocity = collision.relativeVelocity.magnitude;
                AddStickyJoint(collision);
                
                if (debugProperties.OutputHitVelocity)
                    Debug.Log(string.Format("'{0}' Hit Velocity: {1}", this.name, velocity));
            }
        }

        void OnCollisionStay2D(Collision2D collision)
        {
            //Used to add the joint when collision happens
            if (Enable && !Paused)
            {
                AddStickyJoint(collision);
            }
        }

        //For when the joint breaks
        void OnJointBreak2D(Joint2D joint2D)
        {
            _eventProperties.OnStickBreak.Invoke();

            if (OnStickBreak2D != null)
                OnStickBreak2D(this);

            CleanUp();
        }

        #endregion

        #region Physics / Collision

        private void AddStickyJoint(Collision2D whatIHit)
        {
            if (CollisionValidation(whatIHit))
            {
                ConnectedObjectData connectedObjectData = new ConnectedObjectData();
                Rigidbody2D whatIhitRigidbody = whatIHit.collider.attachedRigidbody;
                if (_parentType == SSS.ParentType.ParentOnStick)
                {
                    if (whatIhitRigidbody == null)
                    {
                        connectedObjectData.Parent = whatIHit.gameObject.transform.parent;
                    }
                    else
                    {
                        connectedObjectData.Parent = whatIhitRigidbody.transform.parent;
                        connectedObjectData._gameObject = whatIhitRigidbody.gameObject;
                    }
                }
                else
                    connectedObjectData._gameObject = whatIHit.gameObject;
                
                connectedObjectData._colliders = whatIHit.gameObject.GetComponentsInChildren<Collider2D>();
                
                Vector2 localAnchor = this.gameObject.transform.worldToLocalMatrix.MultiplyPoint(whatIHit.contacts[0].point);
                Vector2 connectedAnchor = whatIHit.gameObject.transform.worldToLocalMatrix.MultiplyPoint(whatIHit.contacts[0].point);

                ConfigerJoint(connectedObjectData, localAnchor, connectedAnchor, whatIhitRigidbody);
            }
        }
        
        private void AddStickyJoint(Collider2D whatIHit)
        {
            if (CollisionValidation(whatIHit))
            {
                ConnectedObjectData connectedObjectData = new ConnectedObjectData();
                Rigidbody2D whatIhitRigidbody = whatIHit.attachedRigidbody;
                if (_parentType == SSS.ParentType.ParentOnStick)
                {
                    if(whatIhitRigidbody == null)
                    {
                        connectedObjectData.Parent = whatIHit.gameObject.transform.parent;
                    }
                    else
                    {
                        connectedObjectData.Parent = whatIhitRigidbody.transform.parent;
                        connectedObjectData._gameObject = whatIhitRigidbody.gameObject;
                    }
                }
                else
                    connectedObjectData._gameObject = whatIHit.gameObject;
                
                connectedObjectData._colliders = whatIHit.gameObject.GetComponentsInChildren<Collider2D>();

                Vector2 localAnchor =
                    this.gameObject.transform.worldToLocalMatrix.MultiplyPoint(
                        whatIHit.ClosestPoint(this.transform.position));
                Vector2 connectedAnchor = whatIHit.gameObject.transform.worldToLocalMatrix.MultiplyPoint(whatIHit.ClosestPoint(this.transform.position));

                ConfigerJoint(connectedObjectData, localAnchor, connectedAnchor, whatIhitRigidbody);
            }
        }
        
        private void ConfigerJoint(ConnectedObjectData connectedObjectData, Vector2 localAnchor, Vector2 connectedAnchor, Rigidbody2D whatIhitRigidbody)
        {
            Transform newStartParent = connectedObjectData._gameObject.transform.parent;

            if (IgnoreColliders)
            {
                foreach (var myCollider in colliders)
                {
                    foreach (var whatIHitCollider in connectedObjectData._colliders)
                    {
                        Physics2D.IgnoreCollision(myCollider, whatIHitCollider, true);
                    }
                }
            }

            this.transform.Translate(StickOffset);

            _eventProperties.OnStickHandler.Invoke();

            if (OnStick2D != null)
                OnStick2D(this);

            connectedObjectData._gameObject.SendMessage("OnStick2D", this, SendMessageOptions.DontRequireReceiver);

            switch (_jointType)
            {
                case JointType.Fixed:

                    FixedJoint2D fixedJoint = new FixedJoint2D();
                    fixedJoint = this.gameObject.AddComponent<FixedJoint2D>();
                    fixedJoint.anchor = localAnchor;
                    fixedJoint.connectedAnchor = connectedAnchor;

                    fixedJoint.dampingRatio = this._fixedJointProperties.DampingRatio;
                    fixedJoint.frequency = this._fixedJointProperties.Frequency;

                    fixedJoint.enableCollision = this._fixedJointProperties.EnableCollision;
                    if (this._fixedJointProperties.BreakForce != Mathf.Infinity)
                        fixedJoint.breakForce = this._fixedJointProperties.BreakForce;
                    if (this._fixedJointProperties.BreakTorque != Mathf.Infinity)
                        fixedJoint.breakTorque = this._fixedJointProperties.BreakTorque;

                    fixedJoint.connectedBody = whatIhitRigidbody;

                    connectedObjectData._fixedJoint = fixedJoint;

                    break;

                case JointType.Spring:

                    SpringJoint2D springJoint = new SpringJoint2D();
                    springJoint = this.gameObject.AddComponent<SpringJoint2D>();
                    springJoint.anchor = localAnchor;
                    springJoint.connectedAnchor = connectedAnchor;

                    springJoint.distance = this._springJointProperties.Distance;
                    springJoint.dampingRatio = this._springJointProperties.DampingRatio;
                    springJoint.frequency = this._springJointProperties.Frequency;

                    springJoint.enableCollision = this._springJointProperties.EnableCollision;
                    if (this._springJointProperties.BreakForce != Mathf.Infinity)
                        springJoint.breakForce = this._springJointProperties.BreakForce;
                    if (this._springJointProperties.BreakTorque != Mathf.Infinity)
                        springJoint.breakTorque = this._springJointProperties.BreakTorque;

                    springJoint.connectedBody = whatIhitRigidbody;

                    connectedObjectData._springJoint = springJoint;

                    break;

                case JointType.Distance:

                    DistanceJoint2D distanceJoint = new DistanceJoint2D();
                    distanceJoint = this.gameObject.AddComponent<DistanceJoint2D>();
                    distanceJoint.anchor = localAnchor;
                    distanceJoint.connectedAnchor = connectedAnchor;

                    distanceJoint.distance = this._distanceJointProperties.Distance;
                    distanceJoint.maxDistanceOnly = this._distanceJointProperties.MaxDistanceOnly;

                    distanceJoint.enableCollision = this._distanceJointProperties.EnableCollision;
                    if (this._distanceJointProperties.BreakForce != Mathf.Infinity)
                        distanceJoint.breakForce = this._distanceJointProperties.BreakForce;
                    if (this._distanceJointProperties.BreakTorque != Mathf.Infinity)
                        distanceJoint.breakTorque = this._distanceJointProperties.BreakTorque;

                    distanceJoint.connectedBody = whatIhitRigidbody;

                    connectedObjectData._distanceJoint = distanceJoint;

                    break;

                case JointType.Friction:

                    FrictionJoint2D frictionJoint = new FrictionJoint2D();
                    frictionJoint = this.gameObject.AddComponent<FrictionJoint2D>();
                    frictionJoint.anchor = localAnchor;
                    frictionJoint.connectedAnchor = connectedAnchor;

                    frictionJoint.maxForce = this._frictionJointProperties.MaxForce;
                    frictionJoint.maxTorque = this._frictionJointProperties.MaxTorque;

                    frictionJoint.enableCollision = this._frictionJointProperties.EnableCollision;
                    if (this._frictionJointProperties.BreakForce != Mathf.Infinity)
                        frictionJoint.breakForce = this._frictionJointProperties.BreakForce;
                    if (this._frictionJointProperties.BreakTorque != Mathf.Infinity)
                        frictionJoint.breakTorque = this._frictionJointProperties.BreakTorque;

                    frictionJoint.connectedBody = whatIhitRigidbody;

                    connectedObjectData._frictionJoint = frictionJoint;

                    break;

                case JointType.Hinge:

                    HingeJoint2D hingeJoint = new HingeJoint2D();
                    hingeJoint = this.gameObject.AddComponent<HingeJoint2D>();
                    hingeJoint.anchor = localAnchor;
                    hingeJoint.connectedAnchor = connectedAnchor;

                    JointMotor2D jointMotor = new JointMotor2D();
                    jointMotor.motorSpeed = this._hingeJointProperties._motor._motorSpeed;
                    jointMotor.maxMotorTorque = this._hingeJointProperties._motor._maximumMotorForce;
                    hingeJoint.motor = jointMotor;
                    hingeJoint.useMotor = this._hingeJointProperties._useMotor;
                    ///
                    JointAngleLimits2D jointAngleLimits = new JointAngleLimits2D();
                    jointAngleLimits.min = this._hingeJointProperties._angleLimits._lowerAngle;
                    jointAngleLimits.max = this._hingeJointProperties._angleLimits._upperAngle;
                    hingeJoint.limits = jointAngleLimits;
                    hingeJoint.useLimits = this._hingeJointProperties._useLimits;

                    hingeJoint.enableCollision = this._hingeJointProperties.EnableCollision;
                    if (this._hingeJointProperties.BreakForce != Mathf.Infinity)
                        hingeJoint.breakForce = this._hingeJointProperties.BreakForce;
                    if (this._hingeJointProperties.BreakTorque != Mathf.Infinity)
                        hingeJoint.breakTorque = this._hingeJointProperties.BreakTorque;

                    hingeJoint.connectedBody = whatIhitRigidbody;

                    connectedObjectData._hingeJoint = hingeJoint;

                    break;

                case JointType.Relative:

                    RelativeJoint2D relativeJoint = new RelativeJoint2D();
                    relativeJoint = this.gameObject.AddComponent<RelativeJoint2D>();

                    relativeJoint.maxForce = this._relativeJointProperties.MaxForce;
                    relativeJoint.maxTorque = this._relativeJointProperties.MaxTorque;
                    relativeJoint.correctionScale = this._relativeJointProperties.CorrectionScale;
                    relativeJoint.autoConfigureOffset = this._relativeJointProperties.AutoConfigureOffset;
                    relativeJoint.linearOffset = this._relativeJointProperties.LinearOffset;
                    relativeJoint.angularOffset = this._relativeJointProperties.AngularOffset;

                    relativeJoint.enableCollision = this._relativeJointProperties.EnableCollision;
                    if (this._relativeJointProperties.BreakForce != Mathf.Infinity)
                        relativeJoint.breakForce = this._relativeJointProperties.BreakForce;
                    if (this._relativeJointProperties.BreakTorque != Mathf.Infinity)
                        relativeJoint.breakTorque = this._relativeJointProperties.BreakTorque;

                    relativeJoint.connectedBody = whatIhitRigidbody;

                    connectedObjectData._relativeJoint = relativeJoint;

                    break;

                case JointType.Slider:

                    SliderJoint2D sliderJoint = new SliderJoint2D();
                    sliderJoint = this.gameObject.AddComponent<SliderJoint2D>();
                    sliderJoint.anchor = localAnchor;
                    sliderJoint.connectedAnchor = connectedAnchor;

                    sliderJoint.angle = this._sliderJointProperties.Angle;
                    ///
                    JointMotor2D jointMotorSlider = new JointMotor2D();
                    jointMotorSlider.motorSpeed = this._sliderJointProperties._motor._motorSpeed;
                    jointMotorSlider.maxMotorTorque = this._sliderJointProperties._motor._maximumMotorForce;
                    sliderJoint.motor = jointMotorSlider;
                    sliderJoint.useMotor = this._sliderJointProperties._useMotor;
                    ///
                    JointTranslationLimits2D jointAngleLimitsSlider = new JointTranslationLimits2D();
                    jointAngleLimitsSlider.min = this._sliderJointProperties._translateLimits._lowerTranslation;
                    jointAngleLimitsSlider.max = this._sliderJointProperties._translateLimits._upperTranslation;
                    sliderJoint.limits = jointAngleLimitsSlider;
                    sliderJoint.useLimits = this._sliderJointProperties._useLimits;

                    sliderJoint.enableCollision = this._sliderJointProperties.EnableCollision;
                    if (this._sliderJointProperties.BreakForce != Mathf.Infinity)
                        sliderJoint.breakForce = this._sliderJointProperties.BreakForce;
                    if (this._sliderJointProperties.BreakTorque != Mathf.Infinity)
                        sliderJoint.breakTorque = this._sliderJointProperties.BreakTorque;

                    sliderJoint.connectedBody = whatIhitRigidbody;

                    connectedObjectData._sliderJoint = sliderJoint;

                    break;

                case JointType.Target:

                    TargetJoint2D targetJoint = new TargetJoint2D();
                    targetJoint = this.gameObject.AddComponent<TargetJoint2D>();
                    targetJoint.anchor = localAnchor;
                    targetJoint.target = connectedAnchor;

                    targetJoint.maxForce = this._targetJointProperties._maxForce;
                    targetJoint.dampingRatio = this._targetJointProperties._dampingRatio;
                    targetJoint.frequency = this._targetJointProperties._frequency;

                    targetJoint.enableCollision = this._targetJointProperties.EnableCollision;
                    if (this._targetJointProperties.BreakForce != Mathf.Infinity)
                        targetJoint.breakForce = this._targetJointProperties.BreakForce;
                    if (this._targetJointProperties.BreakTorque != Mathf.Infinity)
                        targetJoint.breakTorque = this._targetJointProperties.BreakTorque;

                    targetJoint.connectedBody = whatIhitRigidbody;

                    connectedObjectData._targetJoint = targetJoint;

                    break;

                case JointType.Wheel:

                    WheelJoint2D wheelJoint = new WheelJoint2D();
                    wheelJoint = this.gameObject.AddComponent<WheelJoint2D>();
                    wheelJoint.anchor = localAnchor;
                    wheelJoint.connectedAnchor = connectedAnchor;

                    JointSuspension2D suspension = new JointSuspension2D();
                    suspension.dampingRatio = this._wheelJointProperties._suspensio._dampingRatio;
                    suspension.frequency = this._wheelJointProperties._suspensio._frequency;
                    suspension.angle = this._wheelJointProperties._suspensio._angle;
                    wheelJoint.suspension = suspension;
                    ///
                    JointMotor2D jointMotorWheel = new JointMotor2D();
                    jointMotorWheel.motorSpeed = this._wheelJointProperties._motor._motorSpeed;
                    jointMotorWheel.maxMotorTorque = this._wheelJointProperties._motor._maximumMotorForce;
                    wheelJoint.motor = jointMotorWheel;
                    wheelJoint.useMotor = this._wheelJointProperties._useMotor;

                    wheelJoint.enableCollision = this._wheelJointProperties.EnableCollision;
                    if (this._wheelJointProperties.BreakForce != Mathf.Infinity)
                        wheelJoint.breakForce = this._wheelJointProperties.BreakForce;
                    if (this._wheelJointProperties.BreakTorque != Mathf.Infinity)
                        wheelJoint.breakTorque = this._wheelJointProperties.BreakTorque;

                    wheelJoint.connectedBody = whatIhitRigidbody;

                    connectedObjectData._wheelJoint = wheelJoint;

                    break;

                case JointType.None:
                default:
                    break;
            }

            //Used for when Recursive Infection is checked
            if (StickRecursively)
            {
                if (connectedObjectData._gameObject.GetComponent<SSS2D>() == null)
                {
                    SSS2D stickyStickStuck = connectedObjectData._gameObject.AddComponent<SSS2D>();

                    //Copys all properties to the new StickyStickStuck
                    CopyStickyStickStuck(stickyStickStuck);

                    //Sets the children isInfectParent to false
                    stickyStickStuck.IsInfectParent = false;

                    //Sets the start parent to the new start parent
                    stickyStickStuck.startParent = newStartParent;
                }
            }

            //Sets up the parent
            switch (_parentType)
            {
                case SSS.ParentType.None:
                    break;
                case SSS.ParentType.ChildOnStick:
                    this.transform.SetParent(connectedObjectData._gameObject.transform, true);
                    break;
                case SSS.ParentType.ParentOnStick:
                    connectedObjectData._gameObject.transform.SetParent(this.transform, true);
                    break;
            }

            ConnectedObjectDataLists.Add(connectedObjectData);
        }

        //Check to see if the joint exists for the given GameObject
        private bool CheckIfJointExistsForGameObject(GameObject gameObject)
        {
            foreach (var item in ConnectedObjectDataLists)
            {
                if (item._gameObject == gameObject)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region Functions

        //Checks to see if any of the stuck objects are disabled
        bool CheckForNonEnabledObjects()
        {
            for (int i = 0; i < connectedObjectDataLists.Count; i++)
            {
                if (!connectedObjectDataLists[i]._gameObject.activeInHierarchy)
                {
                    return true;
                }
            }

            return false;
        }

        //This is where it check to see if its ok to add a sticky joint
        private bool CollisionValidation(Collision2D whatIHit)
        {
            //if whatIhit equals null then fail
            if (whatIHit == null)
            {
                return false;
            }

            if(whatIHit.collider.isTrigger)
                return false;
            
            //if whatIhit collider equals the parents collider then fail
            if (whatIHit.collider == this.gameObject.GetComponent<Collider2D>())
            {
                return false;
            }

            //If stickNonRigidbodys is false
            if (!StickToNonRigidbody)
            {
                //If the whatIhit rigidbody equals null then fail
                if (whatIHit.rigidbody == null)
                {
                    return false;
                }
            }

            //If the connected amount of objects is greater than the max sticked size then fail
            if (ConnectedObjectDataLists.Count >= MaxEffectedItems)
            {
                return false;
            }

            //Fail if joint exists
            if (CheckIfJointExistsForGameObject(whatIHit.gameObject))
            {
                return false;
            }

            //If affect infected is checked
            if (!StickOnSticky)
            {
                //If StickyStickStuck exists then fail
                if (whatIHit.gameObject.GetComponent<SSS2D>() != null)
                {
                    return false;
                }
            }

            //If trigger area is active
            if (StickInTriggerArea)
            {
                if (!inTriggerArea)
                    return false;
            }

            if (mainRigidbody == null)
            {
                return false;
            }

            //Checks the stick impact velocity
            if (velocity < StickImpactVelocity)
            {

                return false;
            }

            var whatIHitGameObject = whatIHit.contacts[0].collider.gameObject;

            if (whatIHitGameObject == null)
                return false;

            //Validated by the filter options
            if (!_filterProperties.ValidateFilters(whatIHitGameObject, whatIHit.contacts[0].collider))
                return false;

            return true;
        }

        //This is where it check to see if its ok to add a sticky joint
        private bool CollisionValidation(Collider2D whatIHit)
        {
            //if whatIhit equals null then fail
            if (whatIHit == null)
            {
                return false;
            }

            if(whatIHit.isTrigger)
                return false;
            
            //if whatIhit collider equals the parents collider then fail
            if (whatIHit == this.gameObject.GetComponent<Collider2D>())
            {
                return false;
            }

            //If stickNonRigidbodys is false
            if (!StickToNonRigidbody)
            {
                //If the whatIhit rigidbody equals null then fail
                if (whatIHit.attachedRigidbody == null)
                {
                    return false;
                }
            }

            //If the connected amount of objects is greater than the max sticked size then fail
            if (ConnectedObjectDataLists.Count >= MaxEffectedItems)
            {
                return false;
            }

            //Fail if joint exists
            if (CheckIfJointExistsForGameObject(whatIHit.gameObject))
            {
                return false;
            }

            //If affect infected is checked
            if (!StickOnSticky)
            {
                //If StickyStickStuck exists then fail
                if (whatIHit.gameObject.GetComponent<SSS2D>() != null)
                {
                    return false;
                }
            }

            //If trigger area is active
            if (StickInTriggerArea)
            {
                if (!inTriggerArea)
                    return false;
            }

            if (mainRigidbody == null)
            {
                return false;
            }

            //Checks the stick impact velocity
            if (velocity < StickImpactVelocity)
            {

                return false;
            }

            var whatIHitGameObject = whatIHit.gameObject;

            if (whatIHitGameObject == null)
                return false;

            //Validated by the filter options
            if (!_filterProperties.ValidateFilters(whatIHitGameObject, whatIHit))
                return false;

            return true;
        }
        
        public Vector3 FindClosestPoints(Vector3 point, Collider2D closestCollider)
        {
            Vector3 closestPoint = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

            if (closestCollider != null)
            {
                var distance = Vector3.Distance(point, closestPoint);

                var newPoint = point;

#if !(UNITY_4_6 || UNITY_4_7 || UNITY_4_8 || UNITY_4_9 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4 || UNITY_5_5)
                newPoint = Physics2D.ClosestPoint(point, closestCollider);
#else
                Debug.LogWarning(WarningMessageClosestPoint_5_6);
#endif

                if (distance > Vector3.Distance(point, newPoint))
                {
                    closestPoint = newPoint;
                }
            }
            else
            {
                closestPoint = point;
            }

            return closestPoint;
        }
        
        #endregion

        #region CleanUp

        //Cleans up the mess
        public void CleanUp()
        {
            inTriggerArea = false;

            //Cleans up this objects real parent
            if (startParent == null)
            {
                this.transform.SetParent(null, true);
            }
            else if (startParent.gameObject != null)
            {
                if (startParent.gameObject.activeInHierarchy)
                    this.transform.SetParent(startParent, true);
            }

            //Checks to see of any of the connected objects exist
            if (ConnectedObjectDataLists.Count > 0)
            {
                //Cleans up joins
                CleanupJoints();

                //Cleans up all objects intected by the StickyStickStuck scripted
                InfectCleanUp();
            }

            //Cleans the connected object data list
            ConnectedObjectDataLists.Clear();

            //If not the parent destroy the StickyStickStuck object
            if (!IsInfectParent)
            {
                Destroy(this.GetComponent<SSS2D>());
            }
        }

        //Cleans up all the joints
        private void CleanupJoints()
        {
            foreach (var item in ConnectedObjectDataLists)
            {
                if (item._fixedJoint != null)
                {
                    Destroy(item._fixedJoint);
                }
                if (item._springJoint != null)
                {
                    Destroy(item._springJoint);
                }
                if (item._distanceJoint != null)
                {
                    Destroy(item._distanceJoint);
                }
                if (item._frictionJoint != null)
                {
                    Destroy(item._frictionJoint);
                }
                if (item._hingeJoint != null)
                {
                    Destroy(item._hingeJoint);
                }
                if (item._relativeJoint != null)
                {
                    Destroy(item._relativeJoint);
                }
                if (item._targetJoint != null)
                {
                    Destroy(item._targetJoint);
                }
                if (item._wheelJoint != null)
                {
                    Destroy(item._wheelJoint);
                }
                if (item._sliderJoint != null)
                {
                    Destroy(item._sliderJoint);
                }
            }
        }

        //Cleans up the infected
        private void InfectCleanUp()
        {
            foreach (var item in ConnectedObjectDataLists)
            {
                if (IgnoreColliders)
                {
                    foreach (var myCollider in colliders)
                    {
                        foreach (var whatIHitCollider in item._colliders)
                        {
                            Physics2D.IgnoreCollision(whatIHitCollider, myCollider, false);
                        }
                    }
                }

                if (item._gameObject != null)
                {
                    _eventProperties.OnUnStickHandler.Invoke();

                    if (OnUnStick2D != null)
                        OnUnStick2D(this);

                    item._gameObject.SendMessage("OnUnStick2D", this, SendMessageOptions.DontRequireReceiver);

                    SSS2D stickyStickStuck = item._gameObject.GetComponent<SSS2D>();

                    if (stickyStickStuck != null)
                    {
                        if (stickyStickStuck.IsInfectParent == false)
                        {
                            stickyStickStuck.Enable = false;
                        }
                    }

                    if (_parentType == SSS.ParentType.ParentOnStick)
                        item._gameObject.transform.SetParent(item.Parent, true);
                }
            }
        }

        #endregion
    }
}