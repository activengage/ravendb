using Raven35.Client.Indexes;
using Raven35.Tests.Core.Utils.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raven35.Tests.Core.Utils.Indexes
{
    public class CameraCost : AbstractIndexCreationTask<Camera>
    {
        public CameraCost()
        {
            Map = cameras => from camera in cameras
                             select new
                             {
                                 Id = camera.Id,
                                 Manufacturer = camera.Manufacturer,
                                 Model = camera.Model,
                                 Cost = camera.Cost,
                                 Zoom = camera.Zoom,
                                 Megapixels = camera.Megapixels
                             };
        }
    }
}
