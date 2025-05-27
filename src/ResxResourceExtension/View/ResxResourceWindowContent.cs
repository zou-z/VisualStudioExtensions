using Microsoft.VisualStudio.Extensibility.UI;
using ResxResourceExtension.ViewModel;

namespace ResxResourceExtension.View;

internal class ResxResourceWindowContent()
    : RemoteUserControl(new ResxResourceWindowContentViewModel())
{
}
