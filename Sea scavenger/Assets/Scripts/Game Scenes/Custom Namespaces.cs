using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Custom
{
    namespace SerializationSurrogates
    {
        using System.Runtime.Serialization;

        public static class SerializationSurrogatesUtilities
        {
            public static SurrogateSelector GetSurrogateSelector(System.Type type, ISerializationSurrogate surrogate)
            {
                SurrogateSelector surrogateSelector = new SurrogateSelector();

                surrogateSelector.AddSurrogate(type, new StreamingContext(StreamingContextStates.All),
                    surrogate);

                return surrogateSelector;
            }

            public static SurrogateSelector ChainSurrogates(params SurrogateSelector[] surrogateSelectors)
            {
                SurrogateSelector selector = surrogateSelectors[0];

                for (int i = 1; i < surrogateSelectors.Length; i++)
                {
                    selector.ChainSelector(surrogateSelectors[i]);
                }

                return selector;
            }
        }

        // Author - takatok 
        // Code taken from https://forum.unity.com/threads/vector3-is-not-marked-serializable.435303/
        public class Vector3SerializationSurrogate : ISerializationSurrogate
        {
            public static SurrogateSelector SurrogateSelector => SerializationSurrogatesUtilities.
                GetSurrogateSelector(typeof(Vector3), new Vector3SerializationSurrogate());

            // Method called to serialize a Vector3 object
            public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
            {
                Vector3 v3 = (Vector3)obj;

                info.AddValue("x", v3.x);
                info.AddValue("y", v3.y);
                info.AddValue("z", v3.z);
            }

            // Method called to deserialize a Vector3 object
            public System.Object SetObjectData(System.Object obj, SerializationInfo info,
                                               StreamingContext context, ISurrogateSelector selector)
            {
                Vector3 v3 = (Vector3)obj;

                v3.x = (float)info.GetValue("x", typeof(float));
                v3.y = (float)info.GetValue("y", typeof(float));
                v3.z = (float)info.GetValue("z", typeof(float));
                obj = v3;

                return obj;
            }
        }

        // SalaZar Smile
        public class Vector2SerializationSurrogate : ISerializationSurrogate
        {
            public static SurrogateSelector SurrogateSelector => SerializationSurrogatesUtilities.
                GetSurrogateSelector(typeof(Vector2), new Vector2SerializationSurrogate());

            public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
            {
                Vector2 v2 = (Vector2)obj;

                info.AddValue("x", v2.x);
                info.AddValue("y", v2.y);
            }

            public object SetObjectData(object obj, SerializationInfo info,
                                               StreamingContext context, ISurrogateSelector selector)
            {
                Vector2 v2 = (Vector2)obj;

                v2.x = (float)info.GetValue("x", typeof(float));
                v2.y = (float)info.GetValue("y", typeof(float));

                obj = v2;

                return obj;
            }
        }

        public class Vector2IntSerializationSurrogate : ISerializationSurrogate
        {
            public static SurrogateSelector SurrogateSelector => SerializationSurrogatesUtilities.
                GetSurrogateSelector(typeof(Vector2Int), new Vector2IntSerializationSurrogate());

            public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
            {
                Vector2Int v2int = (Vector2Int)obj;

                info.AddValue("x", v2int.x);
                info.AddValue("y", v2int.y);
            }

            public System.Object SetObjectData(System.Object obj, SerializationInfo info,
                                               StreamingContext context, ISurrogateSelector selector)
            {
                Vector2Int v2int = (Vector2Int)obj;

                v2int.x = (int)info.GetValue("x", typeof(int));
                v2int.y = (int)info.GetValue("y", typeof(int));

                obj = v2int;

                return obj;
            }
        }
        
        public class QuaternionSerializationSurrogate : ISerializationSurrogate
        {
            public static SurrogateSelector SurrogateSelector => SerializationSurrogatesUtilities.
                GetSurrogateSelector(typeof(Quaternion), new QuaternionSerializationSurrogate());

            public void GetObjectData(System.Object obj, SerializationInfo info, 
                StreamingContext context)
            {
                var val = (Quaternion)obj;
                
                info.AddValue("x", val.x);
                info.AddValue("y", val.y);
                info.AddValue("z", val.z);
                info.AddValue("w", val.w);
            }

            public System.Object SetObjectData(System.Object obj, SerializationInfo info,
                StreamingContext context, ISurrogateSelector selector)
            {
                var val = (Quaternion)obj;

                val.x = (float)info.GetValue("x", typeof(float));
                val.y = (float)info.GetValue("y", typeof(float));
                val.z = (float)info.GetValue("z", typeof(float));
                val.w = (float)info.GetValue("w", typeof(float));

                obj = val;

                return obj;
            }
        }
    }
    namespace SmartCoroutines
    {
        using System.Linq;
        using System;

        public abstract class SmartCoroutineBase
        {
            public SmartCoroutineBase(MonoBehaviour owner)
            {
                this.owner = owner;

                isWorking = false;
            }

            protected MonoBehaviour owner;
            private bool isWorking;

            public bool IsWorking
            {
                get
                {
                    return isWorking;
                }
                protected set
                {
                    isWorking = value;
                }
            }

            public abstract void Stop();
        }

        // ***WAITING***
        public sealed class SmartWaitingCoroutine : SmartCoroutineBase
        {
            public SmartWaitingCoroutine(MonoBehaviour mono) : base(mono) { }

            private Coroutine coroutine;

            public void Start(float time, Action methodBefore = null, Action methodAfter = null, 
                Action methodProcess = null)
            {
                Stop();

                IsWorking = true;

                if (methodProcess is null)
                    coroutine = owner.StartCoroutine(WaitingIEnum(time, methodBefore, methodAfter));
                else
                    coroutine = owner.StartCoroutine(WaitingIEnum(time, methodBefore, methodAfter,
                        methodProcess));
            }

            public void StartWaitNextFixedUpdate(Action methodBefore = null, Action methodAfter = null)
            {
                Stop();

                IsWorking = true;

                coroutine = owner.StartCoroutine(WaitNextFixedUpdate(methodBefore, methodAfter));
            }

            public void StartWhile(Func<bool> checkFunc)
            {
                StartWhile(checkFunc, null, null);
            }

            public void StartWhile(Func<bool> checkFunc, Action methodBefore = null, Action methodAfter = null)
            {
                Stop();

                IsWorking = true;

                coroutine = owner.StartCoroutine(WaitingWhileIEnum(checkFunc, methodBefore, methodAfter));
            }

            private IEnumerator WaitingWhileIEnum(Func<bool> checkFunc, Action methodBefore, Action methodAfter)
            {
                methodBefore?.Invoke();

                while (checkFunc.Invoke())
                {
                    yield return null;
                }

                methodAfter?.Invoke();

                IsWorking = false;
            }

            private IEnumerator WaitingIEnum(float time, Action methodBefore, Action methodAfter)
            {
                methodBefore?.Invoke();

                yield return new WaitForSeconds(time);

                methodAfter?.Invoke();

                IsWorking = false;
            }
            
            private IEnumerator WaitingIEnum(float time, Action methodBefore, Action methodAfter,
                Action methodProcess)
            {
                methodBefore?.Invoke();

                float t = 0;

                while ((t += Time.deltaTime) <= time)
                {
                    methodProcess?.Invoke();

                    yield return null;
                }

                methodAfter?.Invoke();

                IsWorking = false;
            }

            private IEnumerator WaitNextFixedUpdate(Action methodBefore, Action methodAfter)
            {
                methodBefore?.Invoke();

                yield return new WaitForFixedUpdate();

                methodAfter?.Invoke();

                IsWorking = false;
            }

            public override void Stop()
            {
                if (IsWorking)
                {
                    owner.StopCoroutine(coroutine);

                    IsWorking = false;
                }
            }
        }

        // ***CACHING***

        // Base class
        public abstract class SmartCoroutineCacheBase<T> : SmartCoroutineBase
            where T : Delegate
        {
            public SmartCoroutineCacheBase(MonoBehaviour owner, T routine) : base(owner)
            {
                this.routine = routine;
            }

            protected T routine;
            protected Coroutine checkStateRoutine;

            public override void Stop()
            {
                if (IsWorking)
                {
                    owner.StopCoroutine(checkStateRoutine);

                    IsWorking = false;
                }
            }
        }

        //Without parameters
        public sealed class SmartCoroutineCache : SmartCoroutineCacheBase<Func<IEnumerator>>
        {
            public SmartCoroutineCache(MonoBehaviour owner, Func<IEnumerator> routine) : base(owner, routine) { }

            public void Start()
            {
                Stop();

                IsWorking = true;

                checkStateRoutine = owner.StartCoroutine(CheckStateRoutine());
            }

            private IEnumerator CheckStateRoutine()
            {
                yield return routine.Invoke();

                IsWorking = false;
            }
        }

        // One parameter
        public sealed class SmartCoroutineCache<TArg> : SmartCoroutineCacheBase<Func<TArg, IEnumerator>>
        {
            public SmartCoroutineCache(MonoBehaviour owner, Func<TArg, IEnumerator> routine) : base(owner, routine) { }

            public void Start(TArg arg)
            {
                Stop();

                IsWorking = true;

                checkStateRoutine = owner.StartCoroutine(CheckStateRoutine(arg));
            }

            private IEnumerator CheckStateRoutine(TArg arg)
            {
                yield return routine.Invoke(arg);

                IsWorking = false;
            }
        }

        // Two parameters
        public sealed class SmartCoroutineCache<TArg1, TArg2> : SmartCoroutineCacheBase<Func<TArg1, TArg2, IEnumerator>>
        {
            public SmartCoroutineCache(MonoBehaviour owner, Func<TArg1, TArg2, IEnumerator> routine) : base(owner, routine) { }

            public void Start(TArg1 arg1, TArg2 arg2)
            {
                Stop();

                IsWorking = true;

                checkStateRoutine = owner.StartCoroutine(CheckStateRoutine(arg1, arg2));
            }

            private IEnumerator CheckStateRoutine(TArg1 arg1, TArg2 arg2)
            {
                yield return routine.Invoke(arg1, arg2);

                IsWorking = false;
            }
        }

        // ***JIT***
        public sealed class SmartCoroutineJIT : SmartCoroutineBase
        {
            public SmartCoroutineJIT(MonoBehaviour owner) : base(owner) { }

            Coroutine coroutine;

            public override void Stop()
            {
                if (IsWorking)
                {
                    owner.StopCoroutine(coroutine);

                    IsWorking = false;
                }
            }

            public void Start(IEnumerator routine)
            {
                Stop();

                IsWorking = true;

                coroutine = owner.StartCoroutine(CheckStateRoutine(routine));
            }

            IEnumerator CheckStateRoutine(IEnumerator routine)
            {
                yield return routine;

                IsWorking = false;
            }
        }

        // ***Multi***
        public abstract class SmartMultipleCoroutineBase<TKey>
        {
            protected SmartMultipleCoroutineBase(MonoBehaviour owner)
            {
                this.owner = owner;
                routines = new Dictionary<TKey, Coroutine>();
            }

            protected MonoBehaviour owner;
            protected Dictionary<TKey, Coroutine> routines;

            public bool IsEmpty
            {
                get
                {
                    return routines.Count == 0;
                }
            }
            public virtual void Stop(TKey key)
            {
                if (!IsEmpty)
                {
                    Coroutine rout;
                    if (routines.TryGetValue(key, out rout))
                    {
                        owner.StopCoroutine(rout);

                        routines.Remove(key);
                    }
                }
            }

            public virtual void StopAll()
            {
                while (!IsEmpty)
                {
                    Stop(routines.First().Key);
                }
            }

            public virtual bool IsWorking(TKey key)
            {
                if (routines.ContainsKey(key))
                    return true;
                return false;
            }
        }

        public sealed class SmartMultipleCoroutine<TKey> : SmartMultipleCoroutineBase<TKey>
        {
            public SmartMultipleCoroutine(MonoBehaviour owner) : base(owner) { }

            public void Start(TKey key, IEnumerator cor)
            {
                Stop(key);

                routines.Add(key, owner.StartCoroutine(CheckStateCoroutine(key, cor)));
            }

            IEnumerator CheckStateCoroutine(TKey key, IEnumerator cor)
            {
                yield return cor;

                routines.Remove(key);
            }
        }

        public sealed class SmartMultipleCoroutine<TKey, TValue> : SmartMultipleCoroutineBase<TKey>
        {
            public SmartMultipleCoroutine(MonoBehaviour owner) : base(owner) { }

            public void Start(TKey key, TValue value, Func<TValue, IEnumerator> rout)
            {
                Stop(key);

                routines.Add(key, owner.StartCoroutine(CheckStateCoroutine(key, value, rout)));
            }

            IEnumerator CheckStateCoroutine(TKey key, TValue value, Func<TValue, IEnumerator> rout)
            {
                yield return rout.Invoke(value);

                routines.Remove(key);
            }
        }

        //public static class StaticSmartCoroutine
        //{
        //    static StaticSmartCoroutine()
        //    {
        //        corsDict = new Dictionary<MonoBehaviour, Coroutine>();
        //    }

        //    private static Dictionary<MonoBehaviour, Coroutine> corsDict;
        //    private bool isWorking;

        //    public bool IsWorking
        //    {
        //        get
        //        {
        //            return isWorking;
        //        }
        //        protected set
        //        {
        //            isWorking = value;
        //        }
        //    }

        //    public static void Stop(MonoBehaviour owner)
        //    {
        //        if (corsDict.ContainsKey(owner))
        //        {

        //        }
        //    }

        //    public static void Start<T>(this MonoBehaviour owner, T rout)
        //        where T : Delegate
        //    {

        //    }

        //    public SmartCoroutineCacheBase(MonoBehaviour owner, T routine) : base(owner)
        //    {
        //        this.routine = routine;
        //    }

        //    protected T routine;
        //    protected Coroutine checkStateRoutine;

        //    public override void Stop()
        //    {
        //        if (IsWorking)
        //        {
        //            owner.StopCoroutine(checkStateRoutine);

        //            IsWorking = false;
        //        }
        //    }

        //    public void Start()
        //    {
        //        Stop();

        //        IsWorking = true;

        //        checkStateRoutine = owner.StartCoroutine(CheckStateRoutine());
        //    }

        //    private IEnumerator CheckStateRoutine()
        //    {
        //        yield return routine.Invoke();

        //        IsWorking = false;
        //    }
        //}
    }
    namespace Patterns
    {
        public class Pool : PoolBase<GameObject>
        {
            public Pool(Transform folder, GameObject prefab, 
                Predicate<GameObject> availablePredicate = null, 
                Action<GameObject> activateAction = null,
                Action<GameObject> releaseAction = null,
                Func<GameObject> createNewFunc = null,
                int startCount = 0, bool folderHasChildren = false) : base( 
                availablePredicate, activateAction, releaseAction, createNewFunc, startCount)
            {
                _availablePredicate = availablePredicate ?? (o => !o.activeSelf);
                _activateAction = activateAction ?? (o => o.SetActive(true));
                _releaseAction = releaseAction ?? (o => o.SetActive(false));
                _createNewFunc = createNewFunc ?? (() => GameObject.Instantiate(prefab, folder));
                
                if (folderHasChildren)
                {
                    for (int i = 0; i < folder.childCount; i++)
                    {
                        _items.Add(folder.GetChild(i).gameObject);
                    }
                }
            }
        }
        
        public class Pool<T> : PoolBase<T> where T : Component
        {
            public Pool(Transform folder, T prefab, 
                Predicate<T> availablePredicate = null, 
                Action<T> activateAction = null,
                Action<T> releaseAction = null,
                Func<T> createNewFunc = null,
                int startCount = 0, bool folderHasChildren = false) : base(
                availablePredicate, 
                activateAction, 
                releaseAction, 
                createNewFunc,
                startCount
                )
            {
                _availablePredicate = availablePredicate ?? (o => !o.gameObject.activeSelf);
                _activateAction = activateAction ?? (o => o.gameObject.SetActive(true));
                _releaseAction = releaseAction ?? (o => o.gameObject.SetActive(false));
                _createNewFunc = createNewFunc ?? (() => GameObject.Instantiate(prefab.gameObject, folder).GetComponent<T>());

                _folder = folder;
                
                if (folderHasChildren)
                {
                    for (int i = 0; i < folder.childCount; i++)
                    {
                        _items.Add(folder.GetChild(i).gameObject.GetComponent<T>());
                    }
                }
            }

            private Transform _folder;

            public T CreateNew(T prefab)
            {
                return CreateNew(() => GameObject.Instantiate(prefab.gameObject, _folder).GetComponent<T>());
            }
        }
        
        public class PoolBase<T>
        {
                public PoolBase(Predicate<T> availablePredicate, 
                Action<T> activateAction,
                Action<T> releaseAction,
                Func<T> createNewFunc = null, int startCount = 0)
            {
                _items = new List<T>();
                _availablePredicate = availablePredicate;
                _activateAction = activateAction;
                _releaseAction = releaseAction;
                _createNewFunc = createNewFunc;

                while (startCount-- > 0)
                {
                    Release(CreateNew());
                }
            }

            protected List<T> _items;
            protected Predicate<T> _availablePredicate;
            protected Action<T> _activateAction;
            protected Action<T> _releaseAction;
            protected Func<T> _createNewFunc;
            
            public T GetAvailableOrNull(Predicate<T> availablePredicate, Action<T> activateAction)
            {
                foreach (var i in _items)
                {
                    if (availablePredicate(i))
                    {
                        activateAction(i);
                        return i;
                    }
                }

                return default;
            }
            
            public T GetAvailableOrNull(Predicate<T> availablePredicate)
            {
                return GetAvailableOrNull(availablePredicate, _activateAction);
            }
            
            public T GetAvailableOrNull()
            {
                return GetAvailableOrNull(_availablePredicate);
            }
            
            public T GetAvailable()
            {
                return GetAvailableOrNull() ?? CreateNew();
            }
            
            public T CreateNew(Func<T> createNewFunc)
            {
                T newObj = createNewFunc();
                _activateAction(newObj);
                _items.Add(newObj);

                return newObj;
            }
            
            public T CreateNew()
            {
                return CreateNew(_createNewFunc);
            }

            public void Release(T obj)
            {
                _releaseAction(obj);
            }

            public void Release(IEnumerable<T> objects)
            {
                foreach (var o in objects)
                {
                    Release(o);
                }
            }

            public void ReleaseAll()
            {
                foreach (var i in _items)
                {
                    Release(i);
                }
            }
        }
        
        public abstract class Singleton<TObj> : MonoBehaviour where TObj : MonoBehaviour
        {
            private static TObj instance;
            public static TObj Instance => instance;

            protected void Awake()
            {
                if (instance == null)
                    instance = GetComponent<TObj>();
                else
                {
                    Destroy(this);

                    return;
                }

                Init();
            }

            protected virtual void Init() { }
        }
    }
    namespace Extensions
    {
        namespace UI
        {
            // http://orbcreation.com

            public static partial class RectTransformExtensions
            {
                public static void SetDefaultScale(this RectTransform trans)
                {
                    trans.localScale = new Vector3(1, 1, 1);
                }
                public static void SetPivotAndAnchors(this RectTransform trans, Vector2 aVec)
                {
                    trans.pivot = aVec;
                    trans.anchorMin = aVec;
                    trans.anchorMax = aVec;
                }

                public static Vector2 GetSize(this RectTransform trans)
                {
                    return trans.rect.size;
                }
                public static float GetWidth(this RectTransform trans)
                {
                    return trans.rect.width;
                }
                public static float GetHeight(this RectTransform trans)
                {
                    return trans.rect.height;
                }

                public static void SetPositionOfPivot(this RectTransform trans, Vector2 newPos)
                {
                    trans.localPosition = new Vector3(newPos.x, newPos.y, trans.localPosition.z);
                }

                public static void SetLeftBottomPosition(this RectTransform trans, Vector2 newPos)
                {
                    trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
                }
                public static void SetLeftTopPosition(this RectTransform trans, Vector2 newPos)
                {
                    trans.localPosition = new Vector3(newPos.x + (trans.pivot.x * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
                }
                public static void SetRightBottomPosition(this RectTransform trans, Vector2 newPos)
                {
                    trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y + (trans.pivot.y * trans.rect.height), trans.localPosition.z);
                }
                public static void SetRightTopPosition(this RectTransform trans, Vector2 newPos)
                {
                    trans.localPosition = new Vector3(newPos.x - ((1f - trans.pivot.x) * trans.rect.width), newPos.y - ((1f - trans.pivot.y) * trans.rect.height), trans.localPosition.z);
                }

                public static void SetSize(this RectTransform trans, Vector2 newSize)
                {
                    Vector2 oldSize = trans.rect.size;
                    Vector2 deltaSize = newSize - oldSize;
                    trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
                    trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
                }

                public static void SetSize(this RectTransform trans, float x, float y)
                {
                    SetSize(trans, new Vector2(x, y));
                }
                public static void SetWidth(this RectTransform trans, float newSize)
                {
                    SetSize(trans, new Vector2(newSize, trans.rect.size.y));
                }
                public static void SetHeight(this RectTransform trans, float newSize)
                {
                    SetSize(trans, new Vector2(trans.rect.size.x, newSize));
                }
            }

            // my
            public static partial class RectTransformExtensions
            {
                public static void FitInto(this RectTransform origin, RectTransform destiny)
                {
                    FitInto(origin, destiny.GetSize());
                }

                public static void FitInto(this RectTransform origin, Vector2 dstSize)
                {
                    Vector2 orSize = origin.GetSize();
                    float ratio = orSize.x / orSize.y;

                    origin.SetSize(dstSize.y * ratio, dstSize.y);

                    orSize = origin.GetSize();

                    if (orSize.x > dstSize.x)
                    {
                        origin.SetSize(dstSize.x, dstSize.x / ratio);
                    }
                }
            }
        }
        namespace Vector
        {
            public static class VectorUtility
            {
                public enum ComponentType
                {
                    X = 0,
                    Y = 1,
                    Z = 2
                }
                public static Vector3 SetComponentValueOfVector(Vector3 vector, ComponentType compType, float value)
                {
                    vector[(int)compType] = value;

                    return vector;
                }

                public static Vector3 SetComponentValue(this Vector3 vector, ComponentType compType, float value)
                {
                    return SetComponentValueOfVector(vector, compType, value);
                }

                public static Vector3 DirectionVector(Vector3 fromPoint, Vector3 toPoint)
                {
                    return DistanceVector(fromPoint, toPoint).normalized;
                }

                public static Vector3 DirectionVectorWithoutComponent(Vector3 fromPoint, Vector3 toPoint, ComponentType comp)
                {
                    return DistanceVectorWithoutComponent(fromPoint, toPoint, comp).normalized;
                }

                public static Vector3 DistanceVector(Vector3 fromPoint, Vector3 toPoint)
                {
                    return toPoint - fromPoint;
                }

                public static Vector3 DistanceVectorWithoutComponent(Vector3 fromPoint, Vector3 toPoint, ComponentType comp)
                {
                    fromPoint[(int)comp] = 0;
                    toPoint[(int)comp] = 0;

                    return DistanceVector(fromPoint, toPoint);
                }

                public static float DistanceWithoutComponent(Vector3 p1, Vector3 p2, ComponentType comp)
                {
                    return DistanceVectorWithoutComponent(p1, p2, comp).magnitude;
                }

                public static float CircledAngle(Vector3 from, Vector3 to, Vector3 axis)
                {
                    float _angle = Vector3.SignedAngle(from, to, axis);
                    if (_angle < 0)
                        _angle += 360f;

                    return _angle;
                }

                public static Vector2Int Vec2ToVec2Int(Vector2 point, int gridSize)
                {
                    return new Vector2Int(Mathf.RoundToInt(point.x / gridSize), Mathf.RoundToInt(point.y / gridSize)) * gridSize;
                }
            }
        }
        namespace Linq
        {
            using System;

            public static class IEnumerableLinqExtension
            {
                /// <typeparam name="T"></typeparam>
                /// <returns></returns>
                public static T ElementWithMin<T>(this IEnumerable<T> col, Func<T, float> selector)
                {
                    IEnumerator<T> ienum = col.GetEnumerator();
                    ienum.Reset();
                    ienum.MoveNext();
                    float funcV = selector(ienum.Current);
                    var minT = (Element: ienum.Current, Value: funcV);

                    while (ienum.MoveNext())
                    {
                        funcV = selector(ienum.Current);
                        if (minT.Value > funcV)
                            minT = (ienum.Current, funcV);
                    }

                    return minT.Element;
                }

                public static T Random<T>(this IEnumerable<T> col)
                {
                    int selectedInd = UnityEngine.Random.Range(0, col.Count());
                    int ind = 0;

                    foreach (var e in col)
                    {
                        if (ind++ == selectedInd)
                        {
                            return e;
                        }
                    }

                    return default(T);
                }
            }
        }
        namespace Rect
        {
            public static class RectExtension
            {
                public static Vector2 GetRandomPoint2D(this UnityEngine.Rect r)
                {
                    Vector2 relBoundsRandPoint = r.size;

                    relBoundsRandPoint.x *= UnityEngine.Random.Range(0, 1f);
                    relBoundsRandPoint.y *= UnityEngine.Random.Range(0, 1f);

                    return r.min + relBoundsRandPoint;
                }
            }
        }
    }
    #if UNITY_EDITOR
    namespace ToolsUtilities
    {
        using UnityEditor;

        public static class CustomToolsUtility
        {
            static CustomToolsUtility()
            {
                savedPathsDict = new Dictionary<System.Type, string>();
            }

            private static Dictionary<System.Type, string> savedPathsDict;

            public static bool StickyButton(ref bool state, string text)
            {
                bool tmp = state;

                state = GUILayout.Toggle(state, text, "Button");

                if (tmp == state)
                    return false;
                return true;
            }

            public static void SaveEditorWindow(EditorWindow w)
            {
                var data = JsonUtility.ToJson(w);

                savedPathsDict[w.GetType()] = data;

                EditorPrefs.SetString(w.GetType().ToString() + "_tmp_data", data);
            }

            public static bool LoadEditorWindow(EditorWindow w)
            {
                if (!savedPathsDict.ContainsKey(w.GetType()) ||
                    !EditorPrefs.HasKey(w.GetType().ToString() + "_tmp_data"))
                    return false;

                JsonUtility.FromJsonOverwrite(savedPathsDict[w.GetType()], w);

                return true;
            }
        }
    }
    #endif
}
