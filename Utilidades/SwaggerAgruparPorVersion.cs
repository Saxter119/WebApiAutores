using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace webAPIAutores.Utilidades
{
    public class SwaggerAgruparPorVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var nameSpaceController = controller.ControllerType.Namespace;//Controllers.v1
            var version = nameSpaceController.Split(".").Last().ToLower();//v1
            controller.ApiExplorer.GroupName = version;

    }
    }
}
