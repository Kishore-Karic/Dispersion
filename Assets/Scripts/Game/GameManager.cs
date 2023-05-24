using Dispersion.GenericSingleton;

namespace Dispersion.Game
{
    public class GameManager : GenericSingleton<GameManager>
    {
        public int weapon { get; private set; }

        public void SelectWeapon(int index)
        {
            weapon = index;
        }
    }
}