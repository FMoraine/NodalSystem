using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using System;
using System.Collections;
using UnityEngine;

namespace NodalInteractiveCreator.Objects
{
    public abstract class TouchableElement : NodalBehaviour
    {
        public bool OnActiveScript { get; set; }
        public bool _interactable = true;

        public AudioClip _audioClipDefault = null;
        public float _volume = 1;

        protected Camera _mainCamera = null;
        protected Collider _objectCollider = null;
        protected Transform _objectTransform = null;
        protected AudioSource _objectAudio = null;
        protected bool _audioToValidatePlayed;

        private bool _selected = false;

        protected virtual void Awake()
        {
            if (null != GetComponent<Transform>())
                _objectTransform = GetComponent<Transform>();

            if (null != GetComponent<Collider>())
            {
                _objectCollider = GetComponent<Collider>();
                _objectCollider.isTrigger = true;
            }

            if (null != GetComponent<AudioSource>())
            {
                _objectAudio = GetComponent<AudioSource>();
                _objectAudio.enabled = false;
                _objectAudio.clip = _audioClipDefault;
            }

            if (GetComponents<TouchableElement>().Length > 1)
                GetComponents<TouchableElement>()[0].OnActiveScript = true;
            else
                OnActiveScript = true;
        }

        public virtual void SelectObject(Camera Camera, Vector3 InputPosition)
        {
            if (!_interactable)
                return;

            _selected = true;
            _mainCamera = Camera;
        }

        public virtual void MoveObject(Vector3 InputPosition)
        {
            if (!_interactable)
                return;
        }

        public virtual void DeselectObject()
        {
            if (!_interactable)
                return;

            _selected = false;
        }

        public bool IsSelected()
        {
            return _selected;
        }

        protected virtual IEnumerator PlayAudioToValidate(AudioClip audio)
        {
            if (_objectAudio != null && audio != null)
            {
                if (!_audioToValidatePlayed)
                {
                    _objectAudio.volume = _volume;
                    _objectAudio.clip = audio;
                    _objectAudio.loop = false;
                    _objectAudio.enabled = true;
                    _objectAudio.Play();
                    _audioToValidatePlayed = true;

                    if (_objectAudio.clip != null)
                        yield return new WaitForSeconds(_objectAudio.clip.length);

                    //_objectAudio.enabled = false;
                }
                StopCoroutine(PlayAudioToValidate(audio));
            }
            else
                StopCoroutine(PlayAudioToValidate(audio));
        }

        protected IEnumerator FadeOutVolumeAudio()
        {
            if (_objectAudio != null)
            {
                float velocity = 0;

                while (_objectAudio.volume > 0.01f)
                {
                    _objectAudio.volume = Mathf.SmoothDamp(_objectAudio.volume, 0, ref velocity, .1f);
                    yield return null;
                }

                _objectAudio.volume = 0;
            }

            StopCoroutine(FadeOutVolumeAudio());
        }
    }
}
