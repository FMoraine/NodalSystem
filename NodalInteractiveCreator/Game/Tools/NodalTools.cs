using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Attributes;
using Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools.Nodal;
using UnityEngine;

namespace Assets._GENERAL._SCRIPTS.NodalInteractiveCreator.Scripts.Tools
{
    public class NodalTools
    {

        public static List<T> GetAllConnections<T>(object o, bool refreshGUID = false) where T : NodeConnector
        {
            List<T> n = new List<T>();
            if (o == null)
            {
                Debug.LogError("No object founded");
                return n;
            }
                
          
            FieldInfo[] infos = o.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in infos) 
            {
                if (typeof(T).IsAssignableFrom(field.FieldType))
                {
                    T val = (T)field.GetValue(o);

                    if (val == null)
                    {
                        val = (T)Activator.CreateInstance(field.FieldType);
                        field.SetValue(o, val);
                    }

                    if (string.IsNullOrEmpty(val.guid) || refreshGUID)
                    {
                        val.GenerateGUID();
                    } 
                    n.Add((T)val);
                }
            }
            return n;
        }

        public static NodeStyleAttribute GetStyleOfType(Type t)
        {
           object[] array = t.GetCustomAttributes(typeof(NodeStyleAttribute), true);

            if (array.Length > 0)
                return array[0] as NodeStyleAttribute;

            return new NodeStyleAttribute(t.Name , "");

        }

        public static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }


        public static List<FieldInfo> GetAllConnectionsFields<T>(object o, bool refreshGUID = false) where T : NodeConnector
        {
            List<FieldInfo> n = new List<FieldInfo>();
            if (o == null)
            {
                Debug.LogError("No object founded");
                return n;
            }


            FieldInfo[] infos = o.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in infos)
            {
                if (typeof(T).IsAssignableFrom(field.FieldType))
                {
                    T val = (T)field.GetValue(o);

                    if (val == null)
                    {
                        val = (T)Activator.CreateInstance(field.FieldType);
                        field.SetValue(o, val);
                    }

                    if (string.IsNullOrEmpty(val.guid) || refreshGUID)
                    {
                        val.GenerateGUID();
                    }
                    n.Add(field);
                }
            }
            return n;
        }

        public static List<genericConnectorSerializer> GetAllGenericConnections<T>(object o, bool refreshGUID = false) where T : NodeConnector
        {
            List<genericConnectorSerializer> n = new List<genericConnectorSerializer>();
            if (o == null)
            {
                Debug.LogError("No object founded");
                return n;
            }


            FieldInfo[] infos = o.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in infos)
            {
                if (typeof(T).IsAssignableFrom(field.FieldType) && field.FieldType.IsGenericType)
                {
                    T val = (T)field.GetValue(o);
                    if (val == null)
                    {
                        val = (T)Activator.CreateInstance(field.FieldType);
                        field.SetValue(o, val);
                    }

                    if (string.IsNullOrEmpty(val.guid) || refreshGUID)
                    {
                        val.GenerateGUID();
                    }

                    n.Add(new genericConnectorSerializer(){fieldName = field.Name , guid = val.guid });
                }
            }
            return n;
        }

        [Serializable]
        public struct genericConnectorSerializer
        {
            public string fieldName;
            public string guid;
        }

        public static void FillGenericsConnections(object o,
            List<genericConnectorSerializer> gL)
        {
           
            Type t = o.GetType();
            foreach (var g in gL)
            {
                FieldInfo f = t.GetField(g.fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
               
                if (f != null)
                {
                    NodeConnector n = (NodeConnector)f.GetValue(o);
                    if (n == null)
                        n = (NodeConnector)Activator.CreateInstance(f.FieldType);
                    
                    n.ForceAffectGUID(g.guid);

                    f.SetValue(o , n);
                }
            }
        }

        public static List<FieldInfo> GetFieldsByAttribute<TAttribute>( object o) where TAttribute : NodalField
        {
            return GetFieldsByAttribute<TAttribute>(o.GetType());
        }

        public static List<FieldInfo> GetFieldsByAttribute<TAttribute>(Type t) where TAttribute : NodalField
        {
            List<FieldInfo> l = new List<FieldInfo>();

            FieldInfo[] infos = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            foreach (var field in infos)
            {
                object[] attri = field.GetCustomAttributes(false);
                for (int i = 0; i < attri.Length; i++)
                {
                    if (attri[i] is TAttribute)
                        l.Add(field);
                }
            }

            return l;
        }


    }
}
 