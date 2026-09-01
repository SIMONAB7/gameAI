# 🎮 gameAI — Procedural Object Placement Prototype

A small Unity/C# prototype that experiments with procedural terrain-based object spawning and rule-driven placement.

**Repository:** https://github.com/SIMONAB7/gameAI

---

## 📖 About the Project

`gameAI` is a Unity prototype focused on automatically placing a player and different world objects across procedurally generated terrain.

The project uses a C# `objectGenerator` script to wait for terrain generation and then spawn several object types according to different placement rules. Instead of placing every object completely at random, the script evaluates the generated terrain and applies simple conditions such as low elevation, high elevation, isolation, proximity to other objects, or a steep-terrain rule.

This is a relatively small experimental project rather than a complete game, but it demonstrates procedural generation ideas, event-driven behaviour and rule-based world population in Unity.

---

## ✨ Main Features

### 🌍 Terrain-Aware Object Spawning

The object generator is designed to work alongside a procedural terrain generator.

Once the terrain reports that generation is complete, the system retrieves:

- The generated height map
- Terrain scale information
- The terrain height curve
- Maximum terrain height

It then begins populating the scene.

### 🧍 Player Spawning

The player is spawned at a randomly selected valid terrain position after terrain generation has completed.

### 🧪 Rule-Based Item Placement

Different object types use different placement rules:

| Object | Placement Rule |
| --- | --- |
| Health Potion | Low terrain |
| Coin | Random position |
| Weapon | Steep terrain |
| Bush | Isolated position |
| Rock | High terrain |
| Tree | Near previously tracked objects |
| Player | Random position |

### ⛰️ Height-Based Placement

The height map is used to calculate the world-space height of candidate spawn positions.

Examples include:

- **Low terrain:** below 30% of the maximum terrain height
- **High terrain:** above 70% of the maximum terrain height

### 🌳 Proximity and Isolation Rules

The script keeps track of selected spawn positions so that some objects can be placed according to their distance from existing objects.

Two distance rules are used:

```text
Isolation radius: 15 units
Proximity radius: 10 units
```

This allows the prototype to create both separated and clustered object placement.

### 🔁 Configurable Spawn Count

The number of objects spawned for each type is controlled through:

```csharp
public int objectsPerType = 3;
```

This can be adjusted directly from the Unity Inspector.

---

## 🛠️ Technologies

| Area | Technology |
| --- | --- |
| Game Engine | Unity |
| Language | C# |
| Unity Version | 2022.3.45f1 |
| Navigation Package | Unity AI Navigation |
| Scene System | Unity Scenes |
| Programming Style | Component-based / event-driven |

The project also includes standard Unity packages such as TextMeshPro, Timeline, Visual Scripting and UI support.

---

## 🧠 How the Placement Logic Works

After the terrain is generated, the generator follows this general flow:

```text
Terrain Generated
       ↓
Read Height Map
       ↓
Read Terrain Scale
       ↓
Spawn Player
       ↓
Choose Object Type
       ↓
Generate Random X/Z Position
       ↓
Calculate Terrain Height
       ↓
Validate Against Placement Rule
       ↓
Valid?
  ↙           ↘
No             Yes
↓               ↓
Try Again    Spawn Object
```

The generator continues choosing positions until one satisfies the rule assigned to that object.

---

## 🧩 Main Script

### `objectGenerator.cs`

This is the main custom C# script included in the project.

It contains references for:

```text
Health Potion
Coin
Weapon
Bush
Rock
Tree
Player
```

It also contains the logic for:

- Waiting for terrain generation
- Reading the generated height map
- Spawning the player
- Spawning multiple object types
- Choosing random terrain positions
- Evaluating height-based rules
- Checking isolation
- Checking proximity
- Instantiating prefabs

The generator subscribes to a terrain-generated event:

```csharp
terrainGenerator.OnTerrainGenerated += OnTerrainGenerated;
```

This means object placement begins only after the procedural terrain is ready.

---

## 📁 Project Structure

The main project files are organised approximately as:

```text
gameAI/
│
├── Assets/
│   ├── Scenes/
│   │   └── SampleScene.unity
│   │
│   ├── Sripts/
│   │   └── objectGenerator.cs
│   │
│   └── Materials/
│
├── Packages/
│   ├── manifest.json
│   └── packages-lock.json
│
├── ProjectSettings/
│
└── My project.sln
```

> Note: the folder is named `Sripts` in the current project rather than `Scripts`.

---

## 🎬 Scene

The project contains a Unity scene:

```text
Assets/Scenes/SampleScene.unity
```

The scene includes objects such as:

```text
Directional Light
mapGenerator
spawner
```

The `spawner` is responsible for using the object-generation behaviour.

---

## 💻 Running the Project

### Requirements

The project was created using:

```text
Unity 2022.3.45f1
```

Using the same Unity version is recommended.

### 1. Clone the repository

```bash
git clone https://github.com/SIMONAB7/gameAI.git
cd gameAI
```

### 2. Open Unity Hub

Select:

```text
Add project from disk
```

and choose the cloned project folder.

### 3. Open the Scene

Open:

```text
Assets/Scenes/SampleScene.unity
```

### 4. Enter Play Mode

Press the Unity **Play** button to run the scene.

---

## ⚠️ Current Prototype Limitations

This repository appears to be an experimental/in-progress Unity prototype.

The supplied `objectGenerator.cs` references a class named:

```csharp
ProceduralTerrainGenerator
```

but the source file defining that class is not included in the uploaded project archive.

Because of this, the project may require that missing terrain-generation script or related assets before it can compile and run exactly as intended.

The current `IsSteep()` implementation is also a placeholder:

```csharp
return Random.value > 0.5f;
```

So weapons are not yet placed using a real terrain-gradient calculation.

These limitations reflect the prototype state of the project rather than a finished game system.

---

## 🧠 Programming Concepts Demonstrated

This project demonstrates experience with:

- Unity development
- C# scripting
- MonoBehaviour components
- Procedural generation
- Event-driven programming
- Prefab instantiation
- Unity Inspector references
- Height-map processing
- Vector calculations
- Random generation
- Lists and collections
- Rule-based decision logic
- Distance checking with `Vector3.Distance`
- Scene organisation
- Basic game-world population

---

## 🔮 Possible Improvements

The prototype could be extended by:

- Adding the complete procedural terrain generator
- Replacing the placeholder steepness check with actual terrain-gradient analysis
- Preventing different object types from overlapping
- Adding NavMesh-based AI characters
- Introducing enemies or NPCs
- Adding collectible behaviour for coins and health potions
- Creating weapon pickup mechanics
- Generating different biomes
- Using weighted spawn probabilities
- Adding minimum and maximum spawn counts per object
- Improving object clustering rules
- Adding a playable game loop
- Cleaning generated Unity folders from source control
- Adding screenshots or gameplay footage to the README

---

## 🎯 Project Purpose

The main purpose of the project is to experiment with **procedural world generation and rule-based object placement in Unity**.

Rather than manually positioning every object, the system attempts to populate generated terrain automatically according to environmental conditions.

It serves as a useful prototype for learning how procedural generation, terrain information and spawning systems can work together in a 3D Unity environment.

---

## 👩‍💻 Author

**Simona Bosilkova**

GitHub: https://github.com/SIMONAB7
If any questions please feel free to contact me! LinkedIn: www.linkedin.com/in/simona-bosilkova-38b52b25a
