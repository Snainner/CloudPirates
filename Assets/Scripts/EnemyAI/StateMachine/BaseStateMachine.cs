using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

namespace FSM
{
    public class BaseStateMachine : MonoBehaviour
    {
        [SerializeField]
        BaseState _initState;

        Dictionary<Type, Component> _cachedComponents;
        public BaseState CurrentState { get; set; }
        public bool IsPlayerSighted { get; private set; }
        float _attentionTime = 5f;
        Coroutine _CurrentAttention;
        private void Awake()
        {
            CurrentState = _initState;
            _cachedComponents = new Dictionary<Type, Component>();
        }

        private void OnEnable()
        {
            EventManager.AddListener<PlayerSightedEvent>(OnPlayerSighted);
            IsPlayerSighted = false;
        }
        private void OnDisable()
        {
            EventManager.RemoveListener<PlayerSightedEvent>(OnPlayerSighted);
        }
        private void Update()
        {
            CurrentState.Execute(this);
        }

        //Caches the returned component of base in the state machine for cheaper retrieval 
        public new T GetComponent<T>() where T : Component
        {
            if (_cachedComponents.ContainsKey(typeof(T))) 
                return _cachedComponents[typeof(T)] as T;

            var component = base.GetComponent<T>();
            if(component != null)
            {
                _cachedComponents.Add(typeof(T), component);
            }
                return component;

        }

        void OnPlayerSighted(PlayerSightedEvent e)
        {
            IsPlayerSighted = e.PlayerSighted;
            
            if (_CurrentAttention != null)
            {
                StopCoroutine( _CurrentAttention );
                _CurrentAttention = StartCoroutine(AttentionSpan());
            }
            _CurrentAttention = StartCoroutine(AttentionSpan());

            //start timer to set this to false
            //each time the event gets triggered reset the timer 

        }

        IEnumerator AttentionSpan()
        {
            yield return new WaitForSeconds(_attentionTime);
            IsPlayerSighted = false;

        }
    }
}
