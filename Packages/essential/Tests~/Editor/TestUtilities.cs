using System;
using System.Collections.Generic;
using System.Linq;
using EntitiesBT.Components;
using EntitiesBT.Core;
using NUnit.Framework;
using UnityEngine;
using Utilities = EntitiesBT.Components.Utilities;

namespace EntitiesBT.Test
{
    [TestFixture]
    public class TestUtilities
    {
        class TestComponent : MonoBehaviour {}

        private GameObject[] _objects;
        private string[] _objectNamesWithoutT;
        private readonly string[] _objectNames = {
            "0T",
                "00T",
                    "000",
                    "001T",
                "01T",
                    "010",
                        "0100T",
                        "0101T",
                        "0102T",
                        "0103T",
                    "011T",
                    "012",
                "02",
                    "020T",
                        "0200T",
                        "0201T",
                        "0202T",
                    "021T",
                    "022T"
        };

        [SetUp]
        public void Setup()
        {
            _objects = new GameObject[_objectNames.Length];
            _objectNamesWithoutT = _objectNames.Select(ObjName).ToArray();
            for (var i = 0; i < _objectNames.Length; i++) NewGameObject(i);
            
            void NewGameObject(int index)
            {
                var name = _objectNames[index];
                var hasTestComponent = name.EndsWith("T");
                name = ObjName(name);
                var obj = new GameObject(name);
                if (hasTestComponent) obj.AddComponent<TestComponent>();
                
                var parentName = name.Substring(0, name.Length - 1);
                var parentIndex = Array.IndexOf(_objectNamesWithoutT, parentName);
                if (parentIndex >= 0)
                {
                    var parent = _objects[parentIndex];
                    obj.transform.SetParent(parent.transform);
                }

                _objects[index] = obj;
            }
        }

        IEnumerable<string> Children(string parentName)
        {
            return _objectNamesWithoutT
                .Where(name => name.Length == parentName.Length + 1 && name.StartsWith(parentName));
        }
        
        IEnumerable<string> ChildrenWithT(string parentName)
        {
            return _objectNames.Where(name => name.EndsWith("T"))
                .Select(ObjName)
                .Where(name => name.Length == parentName.Length + 1 && name.StartsWith(parentName))
            ;
        }

        IEnumerable<string> Descendants(string rootName)
        {
            return rootName.Yield().Concat(
                _objectNamesWithoutT.Where(name => name.Length > rootName.Length && name.StartsWith(rootName))
            );
        }
        
        IEnumerable<string> DescendantsWithT(string rootName)
        {
            return rootName.Yield().Concat(ChildrenWithT(rootName).SelectMany(DescendantsWithT));
        }
        
        string ObjName(string name) => name.EndsWith("T") ? name.Substring(0, name.Length - 1) : name;
        
        [Test]
        public void should_get_all_children_of_parent_gameobject()
        {
            for (var i = 0; i < _objectNames.Length; i++)
            {
                var obj = _objects[i];
                var objName = _objectNamesWithoutT[i];
                
                var childrenNames = Children(objName).ToArray();
                var objectNames = obj.Children().Select(o => o.name).ToArray();
                
                Assert.AreEqual(objectNames, childrenNames);
            }
        }
        
        [Test]
        public void should_get_all_descendants_of_root_gameobject()
        {
            for (var i = 0; i < _objectNames.Length; i++)
            {
                var obj = _objects[i];
                var objName = _objectNamesWithoutT[i];
                
                var childrenNames = Descendants(objName).ToArray();
                var objectNames = obj.Flatten(Utilities.Children).Select(o => o.Value.name).ToArray();
                
                Assert.AreEqual(objectNames, childrenNames);
            }
        }
        
        [Test]
        public void should_get_all_children_with_certain_component_of_parent_gameobject()
        {
            for (var i = 0; i < _objectNames.Length; i++)
            {
                var obj = _objects[i];
                var objName = _objectNamesWithoutT[i];
                
                var childrenNames = ChildrenWithT(objName).ToArray();
                var objectNames = obj.Children<TestComponent>().Select(o => o.name).ToArray();
                
                Assert.AreEqual(objectNames, childrenNames);
            }
        }
        
        [Test]
        public void should_get_all_descendants_with_certain_component_of_root_gameobject()
        {
            for (var i = 0; i < _objectNames.Length; i++)
            {
                var obj = _objects[i].GetComponent<TestComponent>();
                var objName = _objectNamesWithoutT[i];
                
                var childrenNames = _objectNames[i].EndsWith("T") ? DescendantsWithT(objName).ToArray() : new string[0];
                var objectNames = obj == null ? new string[0] : obj.Flatten(Utilities.Children).Select(o => o.Value.name).ToArray();
                
                Assert.AreEqual(objectNames, childrenNames);
            }
        }
        
    }
}
