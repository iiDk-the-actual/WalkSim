namespace WalkSim
{
    public static class Sounds
    {
        public static void Play(int sound, float volume = 0.1f)
        {
            GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(sound, false, volume);
        }
    }
}