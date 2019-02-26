// Machinika Museum
// © Littlefield Studio
//
// Writted by Franck-Olivier FILIN - 2017
//
// RotatableObject.cs

using System.Collections;
using UnityEngine;

namespace NodalInteractiveCreator.Tools
{
    [System.Serializable]
    public class Check
    {
        [SerializeField]
        private int _id;
        public int Id { get { return _id; } set { _id = value; } }

        [SerializeField]
        private float _angle;
        public float Angle { get { return _angle; } set { _angle = value; } }

        [SerializeField]
        private float _distance;
        public float Distance { get { return _distance; } set { _distance = value; } }

        [SerializeField]
        private float _limitSnapMin;
        public float LimitSnapMin { get { return _limitSnapMin; } set { _limitSnapMin = value; } }

        [SerializeField]
        private float _limitSnapMax;
        public float LimitSnapMax { get { return _limitSnapMax; } set { _limitSnapMax = value; } }

        [SerializeField]
        private bool _block;
        public bool Block { get { return _block; } set { _block = value; } }

        [SerializeField]
        private AudioClip _audioCheck;
        public AudioClip AudioCheck { get { return _audioCheck; } set { _audioCheck = value; } }

        [SerializeField]
        private bool _activeCheckOnMove = true;
        public bool ActiveCheckOnMove { get { return _activeCheckOnMove; } set { _activeCheckOnMove = value; } }

        private bool _audioPlayed = true;
        public bool AudioPlayed { get { return _audioPlayed; } set { _audioPlayed = value; } }

        public Check()
        {
        }

        public Check(int id, float angle, float distance, float min, float max)
        {
            Id = id;
            Angle = angle;
            Distance = distance;
            LimitSnapMin = min;
            LimitSnapMax = max;
        }

        public IEnumerator PlayAudioToValidate(AudioSource objectAudio, float volume)
        {
            if (objectAudio != null && objectAudio.clip != null && !AudioPlayed)
            {
                if (objectAudio.enabled)
                {
                    objectAudio.Stop();
                    objectAudio.Play();
                }
                else
                    objectAudio.enabled = true;

                objectAudio.volume = volume;
                objectAudio.loop = false;

                AudioPlayed = true;

                yield return new WaitForSeconds(objectAudio.clip.length);
                //objectAudio.enabled = false;
            }
            //else
            //    objectAudio.enabled = false;
        }
    }
}
