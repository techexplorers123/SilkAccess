#!/usr/bin/env python3
import subprocess, sys, re

if len(sys.argv) < 2:
    print("Usage: python release.py <version>")
    sys.exit(1)

version = sys.argv[1]

# Update Pluging.cs
with open('Plugin.cs') as f:
    data = f.read()

data = re.sub(r'(\[BepInPlugin\(.*?\,\s.*?\")([\d\.]+)(\"\)\])', rf'\1{version}\3', data)
with open('Plugin.cs', 'w') as f:
    f.write(data)

# Commit, tag, push
subprocess.run(f'git add Plugin.cs && git commit -m "Bump version to {version}"', shell=True, check=True)
subprocess.run(f'git tag -a v{version} -m "Release v{version}"', shell=True, check=True)
subprocess.run(f'git push origin HEAD && git push origin v{version}', shell=True, check=True)

print(f"Version bumped and tagged as v{version}")
