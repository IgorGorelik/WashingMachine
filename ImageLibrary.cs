using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WashingMachine
{
    public sealed class ImageLibrary
    {
        #region Constants
        private const string ImagesFileName = "UnitImages.json";
        #endregion

        #region Properties
        private static readonly Lazy<ImageLibrary> singletonInstance = new Lazy<ImageLibrary>(() => new ImageLibrary());
        public static ImageLibrary Instance => singletonInstance.Value;
        private ConcurrentDictionary<BaseUnit.MachineUnitType, string> unitImageFileNames = default;
        private ConcurrentDictionary<BaseUnit.MachineUnitType, Image> unitImages = new ConcurrentDictionary<BaseUnit.MachineUnitType, Image>();

        public Image this[BaseUnit.MachineUnitType unitType]
        {
            get
            {
                if (!unitImages.ContainsKey(unitType))
                {
                    LoadImage(unitType);
                }

                return unitImages[unitType];
            }
        }
        #endregion

        #region Construction
        public bool Initialize()
        {
            return LoadFromFile();
        }
        #endregion

        #region Methods
        private bool LoadFromFile()
        {
            try
            {
                string jsonFilePath = Path.Combine(Application.StartupPath, ImagesFileName);
                using (var reader = new StreamReader(jsonFilePath))
                {
                    string jsonString = reader.ReadToEnd();
                    unitImageFileNames = JsonConvert.DeserializeObject<ConcurrentDictionary<BaseUnit.MachineUnitType, string>>(jsonString);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        private bool LoadImage(BaseUnit.MachineUnitType unitType)
        {
            try
            {
                var image = Image.FromFile(Path.Combine(Application.StartupPath, "Images", unitImageFileNames[unitType]));
                unitImages[unitType] = image;

                return image != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}
