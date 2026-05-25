namespace Runtime.Interfaces
{
    public interface IAudioService
    {
        void PlaySpinTick();
        void PlayBombSfx();
        void StopBombSfx();
        void PlayRewardSfx();
        void PlayGoldRewardSfx();
        void PlayTakeRewardSfx();
        void PlayCollectSfx();
        void PlayItemPopSfx();
        void PlaySpecialItemOpenSfx();
    }
}
