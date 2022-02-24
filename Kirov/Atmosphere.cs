namespace Kirov;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.Logging;

using NAudio.Wave;

using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

class Atmosphere {
    const string AllSoundsVideo = "qW-VEScQndQ";
    static readonly MemoryStream AllSoundsAudio = new();
    static readonly ConcurrentQueue<bool> news = new();

    static readonly ILogger<Atmosphere> log = Logging.Factory.CreateLogger<Atmosphere>();

    static void Run() {
        DownloadSounds().Wait();
        log.LogInformation("Sounds downloaded");

        while (true) {
            if (!news.TryDequeue(out bool newShip) || !NewsEnabled)
                continue;

            Play(newShip);
        }
    }

    static TimeSpan TS(int minutes, int seconds, int millis)
     => TimeSpan.FromMilliseconds(millis + seconds * 1000 + minutes * 60000);

    [MethodImpl(MethodImplOptions.NoInlining)]
    static void Play(bool up) {
        using var file = new StreamMediaFoundationReader(AllSoundsAudio);
        file.CurrentTime = TS(4, 11, 500);
        using var output = new WaveOutEvent();
        output.Init(file);
        output.Play();
        while (output.PlaybackState == PlaybackState.Playing) {
            Thread.Sleep(100);
        }
    }


    static bool newsEnabled = Environment.GetEnvironmentVariable("NO_NEWS") is null;
    public static bool NewsEnabled {
        get => Volatile.Read(ref newsEnabled);
        set => Volatile.Write(ref newsEnabled, value);
    }

    public static void QueueUp() => news.Enqueue(true);
    public static void QueueDown() => news.Enqueue(false);

    static Thread? newsThread;
    public static void Setup() {
        newsThread = new Thread(Run) {
            IsBackground = true,
        };
        newsThread.Start();
    }

    static async Task DownloadSounds() {
        var youtube = new YoutubeClient();
        var streams = await youtube.Videos.Streams.GetManifestAsync(AllSoundsVideo).ConfigureAwait(false);
        var allSounds = streams.GetAudioOnlyStreams().GetWithHighestBitrate();
        await youtube.Videos.Streams.CopyToAsync(allSounds, AllSoundsAudio).ConfigureAwait(false);
    }
}
