using System.Collections.Generic;
using EasyToolKit.Inspector;
using EasyToolKit.ThirdParty.OdinSerializer;

namespace EasyToolKit.TileWorldPro
{
    public class TileBuildPipline
    {
        [LabelText("地形构建处理器管线")]
        [MetroListDrawerSettings]
        [OdinSerialize, ShowInInspector] private List<ITileBuildProcessor> _processors;

        public void BeforeInstantiateTile(BeforeTileInstantiateParameters parameters)
        {
            foreach (var processor in _processors)
            {
                processor.OnBeforeInstantiateTile(parameters);
            }
        }

        public void AfterInstantiateTile(AfterTileInstantiateParameters parameters)
        {
            foreach (var processor in _processors)
            {
                processor.OnAfterInstantiateTile(parameters);
            }
        }
    }
}