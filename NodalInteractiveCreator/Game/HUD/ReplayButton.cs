using UnityEngine;
using System.Collections;
using NodalInteractiveCreator.HUD;
using NodalInteractiveCreator.Managers;

namespace NodalInteractiveCreator.Inventory
{
    public class ReplayButton : InteractiveCanvas
    {
        public Sprite _iconQuit;
        public Sprite _iconBackInventory;
        public GameObject _infoQuitGame;
        public AnimationClip _animeDB;

        protected override void OnDeselect()
        {
            if (InspectSystem.IsInspecting())
                Inventory.Instance.InspectSystem.CloseItemInspection();
            else
            {
                if (!_infoQuitGame.activeSelf)
                {
                    _infoQuitGame.SetActive(true);
                    InfoBox._infoBoxEnabled = true;
                }
                else
                    CloseGUIQuit();
            }
        }

        public void LoadScene(string nameScene)
        {
            InfoBox._infoBoxEnabled = false;
            GameManager.Instance.LoadScene(nameScene);
        }

        public void CloseGUIQuit()
        {
            if (!_infoQuitGame.GetComponent<Animation>().isPlaying)
                StartCoroutine(CloseBox());
        }

        public IEnumerator CloseBox()
        {
            _infoQuitGame.GetComponent<Animation>()[_animeDB.name].time = _animeDB.length;
            _infoQuitGame.GetComponent<Animation>()[_animeDB.name].speed = -1;
            _infoQuitGame.GetComponent<Animation>().Play(_animeDB.name);

            yield return new WaitWhile(() => _infoQuitGame.GetComponent<Animation>().isPlaying);

            _infoQuitGame.SetActive(false);
            InfoBox._infoBoxEnabled = false;

            _infoQuitGame.GetComponent<Animation>()[_animeDB.name].time = 0;
            _infoQuitGame.GetComponent<Animation>()[_animeDB.name].speed = 1;

            StopAllCoroutines();
        }
    }
}