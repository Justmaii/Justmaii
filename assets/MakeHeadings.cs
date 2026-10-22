// Generates the terminal-style SVG headings in assets/headings/.
// Run from the repo root:  dotnet run assets/MakeHeadings.cs
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;

const string Font = "ui-monospace, SFMono-Regular, Menlo, Consolas, 'Liberation Mono', monospace";
const string Bg = "#0d1117", Border = "#30363d", Bar = "#161b22";
const string Fg = "#c9d1d9", Dim = "#8b949e", Green = "#3fb950", Blue = "#58a6ff", Yellow = "#e3b341";

string outDir = Path.Combine(ScriptDir(), "headings");
string style = $$"""
<style>
  text { font-family: {{Font}}; white-space: pre; }
  .cur { animation: blink 1.1s steps(1) infinite; }
  @keyframes blink { 50% { opacity: 0; } }
</style>
""";

Directory.CreateDirectory(outDir);
Session();
Heading("building", "ls ~/engines");
Heading("languages", "cat languages.txt");
Heading("tools", "cat tools.txt");
Heading("snake", "git log --since=1.year");

(string Text, string Color)[] Prompt(string cmd) =>
    [("mai", Green), ("@mbp ", Green), ("~ ", Blue), ("$ ", Fg), (cmd, Fg)];

string Line(int x, int y, (string Text, string Color)[] parts, int size)
{
    string spans = string.Concat(parts.Select(p => $"<tspan fill=\"{p.Color}\">{WebUtility.HtmlEncode(p.Text)}</tspan>"));
    return $"<text x=\"{x}\" y=\"{y}\" font-size=\"{size}\">{spans}</text>";
}

void Session()
{
    int w = 760, size = 15, lh = 24, top = 64;
    (string, string)[][] rows =
    [
        Prompt("whoami"),
        [("mai", Fg)],
        Prompt("cat about.txt"),
        [("Student. I write chess engines from scratch,", Fg)],
        [("then rewrite them in a faster language.", Fg)],
        [("Three versions so far: C#, C#, C++.", Dim)],
        Prompt("echo uci | ./maiengine"),
        [("id name MaiEngine 3", Fg)],
        [("id author Mai", Fg)],
        [("uciok", Yellow)],
        Prompt(""),
    ];
    int h = top + lh * (rows.Length - 1) + 26;
    var body = rows.Select((r, i) => Line(24, top + i * lh, r, size)).ToList();
    // blinking cursor after the last prompt ("mai@mbp ~ $ " = 12 chars)
    double cx = 24 + 12 * size * 0.602;
    body.Add($"<rect class=\"cur\" x=\"{cx.ToString("0.0", CultureInfo.InvariantCulture)}\" y=\"{top + lh * (rows.Length - 1) - 13}\" " +
             "width=\"9\" height=\"17\" fill=\"" + Fg + "\"/>");
    string title = "mai@mbp: ~";
    string svg = $"""
<svg xmlns="http://www.w3.org/2000/svg" width="{w}" height="{h}" viewBox="0 0 {w} {h}">
{style}
<rect x="0.5" y="0.5" width="{w - 1}" height="{h - 1}" rx="10" fill="{Bg}" stroke="{Border}"/>
<path d="M0.5 32 V10.5 a10 10 0 0 1 10 -10 H{(w - 10.5).ToString(CultureInfo.InvariantCulture)} a10 10 0 0 1 10 10 V32 Z" fill="{Bar}" stroke="{Border}"/>
<circle cx="20" cy="16" r="6" fill="#ff5f57"/><circle cx="40" cy="16" r="6" fill="#febc2e"/><circle cx="60" cy="16" r="6" fill="#28c840"/>
<text x="{(w / 2.0).ToString("0.0", CultureInfo.InvariantCulture)}" y="21" font-size="13" fill="{Dim}" text-anchor="middle">{title}</text>
{string.Join("\n", body)}
</svg>

""";
    File.WriteAllText(Path.Combine(outDir, "session.svg"), svg);
}

void Heading(string name, string cmd)
{
    int size = 18;
    var parts = Prompt(cmd);
    int chars = parts.Sum(p => p.Text.Length);
    int w = (int)(chars * size * 0.602) + 48, h = 54;
    string svg = $"""
<svg xmlns="http://www.w3.org/2000/svg" width="{w}" height="{h}" viewBox="0 0 {w} {h}">
{style}
<rect x="0.5" y="0.5" width="{w - 1}" height="{h - 1}" rx="8" fill="{Bg}" stroke="{Border}"/>
{Line(24, 33, parts, size)}
</svg>

""";
    File.WriteAllText(Path.Combine(outDir, $"{name}.svg"), svg);
}

static string ScriptDir([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;
