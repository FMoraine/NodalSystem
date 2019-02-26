using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using Assets._GENERAL._SCRIPTS.Tools.Serialize;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    [Serializable]
    public class NodalInfos
    {
        public Vector2 pos = new Vector2(Screen.width / 2, Screen.height / 2);
        [SerializeField] public List<MaterialConfig> materialConfig = new List<MaterialConfig>();

        public bool isTheorical {
            get { return subjectTheorical != null; }
        }

        public bool IsEmpty()
        {
            return  (subjectTheorical == null || subjectTheorical.GetName() == "TheoricalNode");
        }
        
        [SerializeField] protected TheoricalNode subjectTheorical;

        [SerializeField] [HideInInspector] private string theoricalData;
        [SerializeField] [HideInInspector] private string theoricalType;
        [SerializeField] [HideInInspector] private List<NodalTools.genericConnectorSerializer> genericsCnnectors = new List<NodalTools.genericConnectorSerializer>();
        [SerializeField] [HideInInspector] private List<SerializerObject> genericLinks = new List<SerializerObject>();


        public INodal Subject {
            get {
                return subjectTheorical;
            }
            set {  subjectTheorical = (TheoricalNode)value;}
        }

        public void BeforeSerialize()
        {
            if (subjectTheorical == null)
                return;

            theoricalData = JsonUtility.ToJson(subjectTheorical);
            genericsCnnectors = NodalTools.GetAllGenericConnections<NodeConnector>(subjectTheorical);
            genericLinks = Serializer.SerializeAllObjects(subjectTheorical);
            theoricalType = subjectTheorical.GetType().AssemblyQualifiedName;
        }

        public void EndSerialize()
        {
            if (theoricalData == "")
                return;

            subjectTheorical = (TheoricalNode)JsonUtility.FromJson(theoricalData, Type.GetType(theoricalType));
            NodalTools.FillGenericsConnections(subjectTheorical, genericsCnnectors);
            Serializer.SetSerializeObject(subjectTheorical, genericLinks);
            genericsCnnectors.Clear();
            genericLinks.Clear();
            theoricalData = "";
        }
    }

}
