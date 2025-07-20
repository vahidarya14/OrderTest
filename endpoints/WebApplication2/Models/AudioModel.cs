namespace OrderApi.Models;


public record AudioModel(string Id, string AudioFile)
{
    public string Title { get; set; }
    public long startFromSecond { get; set; }
    public string TopBorderColor { get; set; } = "#242424";
    //--------------
    /** Audio URL */
    public string url { get; set; } = "/examples/audio/audio.wav";
    /** The height of the waveform in pixels or any CSS value; defaults to 100% */
    public decimal height { get; set; } = 70;
    /** The width of the waveform in pixels or any CSS value; defaults to 100% */
    public decimal width { get; set; } = 300;
    /** The color of the waveform */
    public string waveColor { get; set; } = "#E3E3E3";
    /** The color of the progress mask */
    public string progressColor { get; set; } = "#C2B64C";
    /** The color of the playpack cursor */
    public string cursorColor { get; set; } = "#ddd5e9";
    /** The cursor width */
    public decimal cursorWidth { get; set; } = 2;
    /** Render the waveform with bars like this{ get; set; }= ▁ ▂ ▇ ▃ ▅ ▂ */
    public decimal? barWidth { get; set; }
    /** Spacing between bars in pixels */
    public decimal? barGap { get; set; }
    /** Rounded borders for bars */
    public decimal? barRadius { get; set; }
    /** A vertical scaling factor for the waveform */
    public decimal? barHeight { get; set; }
    /** Vertical bar alignment **/
    public string barAlign { get; set; } = "";
    /** Minimum pixels per second of audio (i.e. zoom level) */
    public decimal minPxPerSec { get; set; } = 1;
    /** Render each audio channel as a separate waveform */
    public bool splitChannels { get; set; } = false;
    /** Stretch the waveform to the full height */
    public bool normalize { get; set; } = false;
    /** Stretch the waveform to fill the container; true by default */
    public bool fillParent { get; set; } = true;
    /** Whether to show default audio element controls */
    public bool mediaControls { get; set; } = true;
    /** Play the audio on load */
    public bool autoplay { get; set; } = false;
    /** Pass false to disable clicks on the waveform */
    public bool interact { get; set; } = true;
    /** Allow to drag the cursor to seek to a new position */
    public bool dragToSeek { get; set; } = false;
    /** Hide the scrollbar */
    public bool hideScrollbar { get; set; } = false;
    /** Audio rate */
    public decimal audioRate { get; set; } = 1;
    /** Automatically scroll the container to keep the current position in viewport */
    public bool autoScroll { get; set; } = true;
    /** If autoScroll is enabled; keep the cursor in the center of the waveform during playback */
    public bool autoCenter { get; set; } = true;
    /** Decoding sample rate. Doesn"t affect the playback. Defaults to 8000 */
    public decimal sampleRate { get; set; } = 8000;
}