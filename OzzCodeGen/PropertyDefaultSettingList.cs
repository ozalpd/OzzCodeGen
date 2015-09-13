using OzzUtils.Savables;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OzzCodeGen
{
    public class PropertyDefaultSettingList : SavableList<PropertyDefaultSetting>
    {
        /// <summary>
        /// Reads a project settings file and creates a PropertyDefaultSettingList instance
        /// </summary>
        /// <param name="fileName">An XML file's path that contains project settings</param>
        /// <returns></returns>
        public static PropertyDefaultSettingList OpenFile(string fileName)
        {
            if (!File.Exists(fileName)) return new PropertyDefaultSettingList();

            StreamReader reader = new StreamReader(fileName);
            XmlSerializer x = new XmlSerializer(typeof(PropertyDefaultSettingList));
            PropertyDefaultSettingList instance = x.Deserialize(reader) as PropertyDefaultSettingList;
            reader.Close();

            var orderedInstance = new PropertyDefaultSettingList();
            var orderedList = instance
                                .OrderBy(s => s.DisplayOrder)
                                .ThenBy(n => n.Name)
                                .ToList<PropertyDefaultSetting>();
            foreach (var setting in orderedList)
            {
                orderedInstance.Add(setting);
            }
            return orderedInstance;
        }
    }
}
