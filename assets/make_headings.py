#!/usr/bin/env python3
"""Generates the terminal-style SVG headings in assets/headings/."""
from html import escape
from pathlib import Path

OUT = Path(__file__).parent / "headings"
FONT = "ui-monospace, SFMono-Regular, Menlo, Consolas, 'Liberation Mono', monospace"
BG, BORDER, BAR = "#0d1117", "#30363d", "#161b22"
FG, DIM, GREEN, BLUE, YELLOW = "#c9d1d9", "#8b949e", "#3fb950", "#58a6ff", "#e3b341"

STYLE = f"""<style>
  text {{ font-family: {FONT}; white-space: pre; }}
  .cur {{ animation: blink 1.1s steps(1) infinite; }}
  @keyframes blink {{ 50% {{ opacity: 0; }} }}
</style>"""


def prompt(cmd):
    return [("mai", GREEN), ("@mbp ", GREEN), ("~ ", BLUE), ("$ ", FG), (cmd, FG)]


def line(x, y, parts, size):
    spans = "".join(f'<tspan fill="{c}">{escape(t)}</tspan>' for t, c in parts)
    return f'<text x="{x}" y="{y}" font-size="{size}">{spans}</text>'


def session():
    w, size, lh, top = 760, 15, 24, 64
    rows = [
        prompt("whoami"),
        [("mai", FG)],
        prompt("cat about.txt"),
        [("Student. I write chess engines from scratch,", FG)],
        [("then rewrite them in a faster language.", FG)],
        [("Three versions so far: C#, C#, C++.", DIM)],
        prompt("echo uci | ./maiengine"),
        [("id name MaiEngine 3", FG)],
        [("id author Mai", FG)],
        [("uciok", YELLOW)],
        prompt(""),
    ]
    h = top + lh * (len(rows) - 1) + 26
    body = [line(24, top + i * lh, r, size) for i, r in enumerate(rows)]
    # blinking cursor after the last prompt ("mai@mbp ~ $ " = 12 chars)
    cx = 24 + 12 * size * 0.602
    body.append(f'<rect class="cur" x="{cx:.1f}" y="{top + lh * (len(rows) - 1) - 13}" '
                f'width="9" height="17" fill="{FG}"/>')
    title = "mai@mbp: ~"
    svg = f"""<svg xmlns="http://www.w3.org/2000/svg" width="{w}" height="{h}" viewBox="0 0 {w} {h}">
{STYLE}
<rect x="0.5" y="0.5" width="{w - 1}" height="{h - 1}" rx="10" fill="{BG}" stroke="{BORDER}"/>
<path d="M0.5 32 V10.5 a10 10 0 0 1 10 -10 H{w - 10.5} a10 10 0 0 1 10 10 V32 Z" fill="{BAR}" stroke="{BORDER}"/>
<circle cx="20" cy="16" r="6" fill="#ff5f57"/><circle cx="40" cy="16" r="6" fill="#febc2e"/><circle cx="60" cy="16" r="6" fill="#28c840"/>
<text x="{w / 2}" y="21" font-size="13" fill="{DIM}" text-anchor="middle">{title}</text>
{chr(10).join(body)}
</svg>
"""
    (OUT / "session.svg").write_text(svg)


def heading(name, cmd):
    size = 18
    parts = prompt(cmd)
    chars = sum(len(t) for t, _ in parts)
    w, h = int(chars * size * 0.602) + 48, 54
    svg = f"""<svg xmlns="http://www.w3.org/2000/svg" width="{w}" height="{h}" viewBox="0 0 {w} {h}">
{STYLE}
<rect x="0.5" y="0.5" width="{w - 1}" height="{h - 1}" rx="8" fill="{BG}" stroke="{BORDER}"/>
{line(24, 33, parts, size)}
</svg>
"""
    (OUT / f"{name}.svg").write_text(svg)


if __name__ == "__main__":
    OUT.mkdir(parents=True, exist_ok=True)
    session()
    heading("building", "ls ~/engines")
    heading("languages", "cat languages.txt")
    heading("tools", "cat tools.txt")
    heading("snake", "git log --since=1.year")
