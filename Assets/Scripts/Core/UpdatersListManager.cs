using System.Collections.Generic;

public class UpdatersListManager
{
    List<Updater> updater = new List<Updater>();

    public void Add (Updater updater) {
        this.updater.Add(updater);
    }

    public void GameUpdate () {
        for (int i = 0; i < updater.Count; i++) {
            if (!updater[i].GameUpdate()) {
                int lastIndex = updater.Count - 1;
                updater[i] = updater[lastIndex];
                updater.RemoveAt(lastIndex);
                i -= 1;
            }
        }
    }
}
