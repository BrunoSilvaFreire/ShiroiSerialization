using System;

namespace Shiroi.Drawing.Drawers {
    public abstract class TypeDrawerProvider {
        public abstract bool Supports(Type type);
        public abstract TypeDrawer Provide(Type type);
    }
}