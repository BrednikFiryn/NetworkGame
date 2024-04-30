using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;

namespace Com.BrednikCompany.TestNetworkGame
{
    /// <summary>
    /// Поле для ввода имени игрока. Если пользователь введет свое имя, оно появится над игроком в игре.
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        // Сохраните ключ PlayerPref, чтобы избежать опечаток
        const string playerNamePrefKey = "playerName";

        private void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();

            if( _inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }

        /// <summary>
        /// Задает имя игрока и сохраняет его в настройках PlayerPrefs для будущих сеансов.
        /// </summary>
        /// <param name="value"></param>
        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                Debug.LogError("Имя игрока является нулевым или незаполненным");
                return;
            }

            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
    }
}
