# UnityProject

Welcome to the UnityProject for CTX. This is a [monorepo](https://en.wikipedia.org/wiki/Monorepo) for all Unity work for CTX.

To keep CTX repo's small, they will only include their appropriate builds and instructions on how to build from the UnityProject monorepo.

## Table of Contents

1. Building the project
1. Project Structure
1. Sub Projects
    1. StarcomUI

## Building the Project

The project is currently standardized on Unity 2021.3.8f1. This is not *required* to build, but may not be stable on other versions. To develop, this version is required.

## Project Structure

The project is structed with folders for basic order. Things like scripts, prefab, scenes, materials, and other assets each have their own folder within `Assets/`. If you download an external asset, please put it into `Assets/MiscAssets/`

For scenes, please name them according to the project they are for. For example, the Starcom CTX has a UI that is made with Unity; as such, the scene is entitled StarcomUI.

For documentation, the subproject must link to it's CTX repo. The CTX repo MUST provide documentation on how to build, configure, and develop the subproject.

## Sub Projects

Below is the list of subprojects held within this monorepo.

1. StarcomUI
    - This subproject is the frontend UI interface for the Starcom CTX.
