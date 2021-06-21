using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;

namespace Custom
{
    namespace Patterns
    {
        public class Pool : PoolBase<GameObject>
        {
            public Pool(Transform folder, GameObject prefab, 
                Predicate<GameObject> availablePredicate = null, 
                Action<GameObject> activateAction = null,
                Action<GameObject> releaseAction = null,
                int startCount = 0, bool folderHasChildren = false) : base( 
                availablePredicate, 
                activateAction, 
                releaseAction, 
                () => GameObject.Instantiate(prefab, folder))
            {
                _availablePredicate = availablePredicate ?? (o => !o.activeSelf);
                _activateAction = activateAction ?? (o => o.SetActive(true));
                _releaseAction = releaseAction ?? (o => o.SetActive(false));
                
                if (folderHasChildren)
                {
                    for (int i = 0; i < folder.childCount; i++)
                    {
                        _items.Add(folder.GetChild(i).gameObject);
                    }
                }

                while (folder.childCount < startCount)
                {
                    Release(CreateNew());
                }
            }
        }
        
        public class Pool<T> : PoolBase<T> where T : Component
        {
            public Pool(Transform folder, T prefab, 
                Predicate<T> availablePredicate = null, 
                Action<T> activateAction = null,
                Action<T> releaseAction = null,
                int startCount = 0, bool folderHasChildren = false) : base(
                availablePredicate, 
                activateAction, 
                releaseAction, 
                () => GameObject.Instantiate(prefab.gameObject, folder).GetComponent<T>()
                )
            {
                _availablePredicate = availablePredicate ?? (o => !o.gameObject.activeSelf);
                _activateAction = activateAction ?? (o => o.gameObject.SetActive(true));
                _releaseAction = releaseAction ?? (o => o.gameObject.SetActive(false));

                _folder = folder;
                
                if (folderHasChildren)
                {
                    for (int i = 0; i < folder.childCount; i++)
                    {
                        _items.Add(folder.GetChild(i).gameObject.GetComponent<T>());
                    }
                }

                while (folder.childCount < startCount)
                {
                    Release(CreateNew());
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
                Func<T> createNewFunc = null)
            {
                _items = new List<T>();
                _availablePredicate = availablePredicate;
                _activateAction = activateAction;
                _releaseAction = releaseAction;
                _createNewFunc = createNewFunc;
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
    namespace Math
    {
        public static class Math
        {
            public const float SIN_45 = 0.70710678118f;
        }
    }

    namespace Utility
    {
        public static class VectorUtility
        {
            public static Vector3 SetY(this Vector3 v, float y)
            {
                v.y = y;

                return v;
            }
        }
    }
}
