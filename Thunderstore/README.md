# Description

## A mod that adds Loadouts to the player's inventory (personal and purchasable). Pre-configured loadouts that can be purchased and personal ones that can be hot-swapped. It's highly configurable. Use your imagination!

`Version checks with itself. If installed on the server, it will kick clients who do not have it installed.`

`This mod uses ServerSync, if installed on the server and all clients, it will sync all configs to client`

`This mod uses a file watcher. If the configuration file is not changed with BepInEx Configuration manager, but changed in the file directly on the server, upon file save, it will sync the changes to all clients.`


---

#### This mod is compatible with Auga.

Anything you do not find in this document will be added to the Wiki Tab for this mod. The wiki is the only thing I can
live update if needed. It will also contain the information found on this page, just in case. If you have any questions,
please feel free to ask *AFTER* reading the documentation.

"But Azu, where is the documentation/wiki?" - You, probably

[![](https://i.imgur.com/yKjqxpC.png)](https://valheim.thunderstore.io/package/Azumatt/RapidLoadouts/wiki/)

The Loadouts button and window are both draggable. You can move them around the screen to your liking. The window/button
will remember their positions between sessions. CTRL + RightClick to drag.

Personal loadouts require items to be equipped to be saved.

<details>
<summary><b>Installation Instructions</b></summary>

***You must have BepInEx installed correctly! I can not stress this enough.***

### Manual Installation

`Note: (Manual installation is likely how you have to do this on a server, make sure BepInEx is installed on the server correctly)`

1. **Download the latest release of BepInEx.**
2. **Extract the contents of the zip file to your game's root folder.**
3. **Download the latest release of RapidLoadouts from Thunderstore.io.**
4. **Extract the contents of the zip file to the `BepInEx/plugins` folder.**
5. **Launch the game.**

### Installation through r2modman or Thunderstore Mod Manager

1. **Install [r2modman](https://valheim.thunderstore.io/package/ebkr/r2modman/)
   or [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager).**

   > For r2modman, you can also install it through the Thunderstore site.
   ![](https://i.imgur.com/s4X4rEs.png "r2modman Download")

   > For Thunderstore Mod Manager, you can also install it through the Overwolf app store
   ![](https://i.imgur.com/HQLZFp4.png "Thunderstore Mod Manager Download")
2. **Open the Mod Manager and search for "RapidLoadouts" under the Online
   tab. `Note: You can also search for "Azumatt" to find all my mods.`**

   `The image below shows VikingShip as an example, but it was easier to reuse the image.`

   ![](https://i.imgur.com/5CR5XKu.png)

3. **Click the Download button to install the mod.**
4. **Launch the game.**

</details>

<details><summary><b>About the YAML</b></summary>

Please note: It's highly recommended that you edit the YAML file with a proper text
editor. [VSCode](https://code.visualstudio.com/download) or [Notepad++](https://notepad-plus-plus.org/downloads/) are
good choices.
Notepad is not a good choice. If you use Notepad, you are making things harder on yourself. You can collapse the YAML
elements in things like Notepad++ or VSCode to make it easier to read. You can also use a YAML validator to make sure
your YAML is valid. This one works [YAML Lint](http://www.yamllint.com/).

The YAML file (`Azumatt.RapidLoadouts_Loadouts.yml`) that is created by the mod allows for configuration of Loadouts.
The entries specify Loadouts (in game items, skills, effects) that can be obtained in the game for a specific price.

Each Loadout is represented by a YAML entry. The entry contains information about the items, skills, or effects that
will be given to the player upon purchase, along with their respective properties such as quantity, quality, and price.

## General Structure of the YAML

- `name`: This is the name of the Loadout that will appear in-game. Make sure this is unique.

- `items`: This is a list of items included in the Loadout. Each item has several properties:
    - `item`: The prefab name of the item.
    - `quality`: The quality level of the item.
    - `stack`: The quantity of this item in the Loadout.
    - `use`: If true, the item will be automatically used (consumed/used/equipped) upon purchase.
    - `hotbarSlot`: The position in the player's hotbar where the item will be placed. If multiple items have the same
      slot, they will be added in the order they appear while swapping each other out in the hotbar. The last item added
      will be the one that wins the slot.

- `skills`: This is a list of skills included in the Loadout. Each skill has several
  properties: `Note: This is not implemented for custom skills yet`
    - `skill`: The name of the skill.
    - `level`: The level of the skill that the player will receive.

- `setEffect`: This is a status effect that will be applied to the player upon purchase. It uses the
  setEffectAsGP setting to determine how to apply this effect. If the setting isn't found, it applies it to the player
  directly by default.

- `setEffectAsGP`: If true, the effect in setEffect will be applied as the player's Guardian Power. If false, the effect
  will be
  applied to the player directly. Be careful doing this, some can't be removed easily (like needing to die/logout/quit
  game)

- `dropCurrent`: If true, the player will drop their current items upon purchase. They will go into a tombstone.

- `price`: The cost of the Loadout in-game.

- `costPrefab`: This specifies an item that will be used as an alternative form of payment. If present, the Loadout can
  be bought with the specified item instead of the default currency found in the configuration file (
  Section: `1 - General` Configuration: `LoadoutCostPrefab`).

## Loadouts

The file contains several preconfigured Loadouts such as:

1. "Give Hammer" – gives a hammer of quality 3, for the price of 10 Fine Wood. Example of the requiredGlobalKey
   property.

2. "AzuMage" – provides a set of armor, a staff, and food items, sets all skills to level 100, and costs 60 Etir. It
   also drops the player's current items.

3. "Potion Pack" – includes minor and medium health meads, costing 50 of the default currency.

4. "Testing Pack" – grants a bronze sword and sets the skill levels of 'Swords' to 16 and 'Run' to 100, for the cost of
   100 Bronze.

5. Several Loadouts provide status effects such as "Status Effect Test", "Goblin Shaman Shield", "Rested Buff level 1",
   and "Corpse Run Buff", with varying costs.

6. There are also Loadouts that boost the player's skill levels: "+10 To Skills", "+20 To Skills", and "Max Out Skills".
   Each Loadout boosts all skills by a certain amount and has its own cost.

Remember to edit the file carefully as incorrect or missing data could result in unexpected behavior or crashes in the
game. Save a backup copy before making any changes.


</details>


<details><summary><b>Default YAML</b></summary> 

```yaml
- name: "Give Hammer"
  items:
    - item: "Hammer"
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 1
  dropCurrent: false
  price: 10
  costPrefab: "FineWood"
  requiredGlobalKey: "defeated_eikthyr"
- name: "AzuMage"
  items:
    - item: "ArmorCarapaceChest"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "ArmorCarapaceLegs"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "HelmetFenring"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "StaffFireball"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "Hammer"
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 2
    - item: "YggdrasilPorridge"
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: "SeekerAspic"
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: "Salad"
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
  skills:
    - skill: "All"
      level: 100
  dropCurrent: true
  price: 50
  costPrefab: "Etir"
- name: "Potion Pack"
  items:
    - item: "MeadHealthMinor"
      quality: 3
      stack: 10
      use: false
      hotbarSlot: 0
    - item: "MeadHealthMedium"
      quality: 3
      stack: 10
      use: false
      hotbarSlot: 0
  dropCurrent: false
  price: 50
- name: "Testing Packs"
  items:
    - item: "SwordBronze"
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 1
  skills:
    - skill: "Swords"
      level: 16
    - skill: "Run"
      level: 100
  dropCurrent: false
  price: 1000
  costPrefab: "Bronze"
- name: "Status Effect Test"
  setEffect: "Potion_eitr_minor"
  setEffectAsGP: true
  price: 45
- name: "Goblin Shaman Shield"
  setEffect: "GoblinShaman_shield"
  setEffectAsGP: false
  price: 60
- name: "Rested Buff level 1"
  setEffect: "Rested"
  price: 15
- name: "Corpse Run Buff"
  setEffect: "CorpseRun"
  price: 15
- name: "+10 To Skills"
  skills:
    - skill: "All"
      level: 10
  dropCurrent: false
  price: 250
- name: "+20 To Skills"
  skills:
    - skill: "All"
      level: 20
  dropCurrent: false
  price: 500
- name: "Max Out Skills"
  skills:
    - skill: "All"
      level: 100
  dropCurrent: false
  price: 5000
- name: Meadows
  items:
    - item: ShieldWood
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 2
    - item: ShieldWoodTower
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 2
    - item: Hammer
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Hoe
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Bow
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: ArmorRagsChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorRagsLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: Honey
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: Raspberry
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: CookedDeerMeat
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: CookedMeat
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: Mushroom
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: ArrowWood
      quality: 1
      stack: 200
      use: true
      hotbarSlot: 0
    - item: AxeStone
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: PickaxeBronze
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: SpearFlint
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: KnifeFlint
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Torch
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: Club
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
  skills:
    - skill: Swords
      level: 1
    - skill: Axes
      level: 1
    - skill: Bows
      level: 1
    - skill: Jump
      level: 1
    - skill: Run
      level: 1
    - skill: Blocking
      level: 1
    - skill: Sneak
      level: 1
    - skill: Swim
      level: 1
    - skill: Spears
      level: 1
    - skill: Pickaxes
      level: 1
    - skill: WoodCutting
      level: 1
    - skill: Knives
      level: 1
    - skill: Clubs
      level: 1
    - skill: Unarmed
      level: 1
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: BlackForest
  items:
    - item: ShieldWood
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 2
    - item: ShieldBoneTower
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 2
    - item: Hammer
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Hoe
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Bow
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: ArmorLeatherChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorLeatherLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetLeather
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeDeerHide
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: Honey
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: CookedDeerMeat
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: CookedMeat
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: Mushroom
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: ArrowFlint
      quality: 1
      stack: 200
      use: true
      hotbarSlot: 0
    - item: AxeFlint
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: PickaxeAntler
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: SpearFlint
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: KnifeFlint
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Torch
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: Club
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
  skills:
    - skill: Swords
      level: 10
    - skill: Axes
      level: 10
    - skill: Bows
      level: 10
    - skill: Jump
      level: 10
    - skill: Run
      level: 10
    - skill: Blocking
      level: 10
    - skill: Sneak
      level: 10
    - skill: Swim
      level: 10
    - skill: Spears
      level: 10
    - skill: Pickaxes
      level: 10
    - skill: WoodCutting
      level: 10
    - skill: Knives
      level: 10
    - skill: Clubs
      level: 10
    - skill: Unarmed
      level: 10
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: Swamps
  items:
    - item: SwordBronze
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 1
    - item: ShieldBronzeBuckler
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 2
    - item: ShieldBoneTower
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: Hammer
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Hoe
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: BowFineWood
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: ArmorBronzeChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorBronzeLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetBronze
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeTrollHide
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CarrotSoup
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: DeerStew
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: BoarJerky
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: QueensJam
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: MinceMeatSauce
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: MeadPoisonResist
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: ArrowBronze
      quality: 1
      stack: 200
      use: true
      hotbarSlot: 0
    - item: ArmorTrollLeatherChest
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: ArmorTrollLeatherLegs
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: HelmetTrollLeather
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: AxeBronze
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: PickaxeBronze
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: SpearBronze
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: Torch
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: AtgeirBronze
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: MaceBronze
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
  skills:
    - skill: Swords
      level: 15
    - skill: Axes
      level: 15
    - skill: Bows
      level: 15
    - skill: Jump
      level: 15
    - skill: Run
      level: 15
    - skill: Blocking
      level: 10
    - skill: Sneak
      level: 10
    - skill: Swim
      level: 5
    - skill: Spears
      level: 15
    - skill: Pickaxes
      level: 10
    - skill: WoodCutting
      level: 20
    - skill: Polearms
      level: 15
    - skill: Knives
      level: 15
    - skill: Clubs
      level: 15
    - skill: Unarmed
      level: 15
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: Mountains
  items:
    - item: SwordIron
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 1
    - item: ShieldIronBuckler
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 2
    - item: ShieldBanded
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: ShieldIronTower
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: Hammer
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Hoe
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: BowHuntsman
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: ArmorIronChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorIronLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetIron
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetRoot
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: ArmorRootChest
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: ArmorRootLegs
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: CapeTrollHide
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: TurnipStew
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: ShocklateSmoothie
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: BlackSoup
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadFrostResist
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: Sausages
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: ArrowIron
      quality: 1
      stack: 200
      use: true
      hotbarSlot: 0
    - item: AxeIron
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: PickaxeIron
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: SpearElderbark
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: Torch
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: AtgeirIron
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: MaceIron
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: KnifeChitin
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
  skills:
    - skill: Swords
      level: 25
    - skill: Axes
      level: 25
    - skill: Bows
      level: 25
    - skill: Jump
      level: 25
    - skill: Run
      level: 25
    - skill: Blocking
      level: 25
    - skill: Sneak
      level: 25
    - skill: Swim
      level: 25
    - skill: Spears
      level: 25
    - skill: Pickaxes
      level: 25
    - skill: WoodCutting
      level: 25
    - skill: Knives
      level: 25
    - skill: Polearms
      level: 25
    - skill: Clubs
      level: 25
    - skill: Unarmed
      level: 25
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: Plains
  items:
    - item: SwordSilver
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 1
    - item: ShieldSilver
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 2
    - item: ShieldSerpentscale
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: Hammer
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Hoe
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: BowDraugrFang
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: ArmorWolfChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorWolfLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetDrake
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeWolf
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: WolfMeatSkewer
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: WolfJerky
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: Eyescream
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: OnionSoup
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: CookedWolfMeat
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: ArrowObsidian
      quality: 1
      stack: 200
      use: true
      hotbarSlot: 0
    - item: BattleaxeCrystal
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: PickaxeIron
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: SpearWolfFang
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: Torch
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: SledgeIron
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: MaceSilver
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: KnifeSilver
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: FistFenrirClaw
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: ArmorFenringChest
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: ArmorFenringLegs
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: HelmetFenring
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
  skills:
    - skill: Swords
      level: 35
    - skill: Axes
      level: 35
    - skill: Bows
      level: 35
    - skill: Jump
      level: 35
    - skill: Run
      level: 35
    - skill: Blocking
      level: 35
    - skill: Sneak
      level: 35
    - skill: Swim
      level: 35
    - skill: Spears
      level: 35
    - skill: Pickaxes
      level: 35
    - skill: WoodCutting
      level: 35
    - skill: Knives
      level: 35
    - skill: Polearms
      level: 35
    - skill: Clubs
      level: 35
    - skill: Unarmed
      level: 35
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: PlainsBoss
  items:
    - item: SwordBlackmetal
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 1
    - item: ShieldBlackmetal
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 2
    - item: ShieldBlackmetalTower
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: Hammer
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Hoe
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: BowDraugrFang
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: ArmorPaddedCuirass
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorPaddedGreaves
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetPadded
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeWolf
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: LoxPie
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: SerpentStew
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: Bread
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: WolfJerky
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: BloodPudding
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: ArrowNeedle
      quality: 1
      stack: 200
      use: true
      hotbarSlot: 0
    - item: ArrowFrost
      quality: 1
      stack: 200
      use: false
      hotbarSlot: 0
    - item: BattleaxeCrystal
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: PickaxeIron
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: SpearWolfFang
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: Torch
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: AtgeirBlackmetal
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: MaceNeedle
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: KnifeBlackMetal
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: AxeBlackMetal
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
  skills:
    - skill: Swords
      level: 45
    - skill: Axes
      level: 45
    - skill: Bows
      level: 45
    - skill: Jump
      level: 45
    - skill: Run
      level: 45
    - skill: Blocking
      level: 45
    - skill: Sneak
      level: 45
    - skill: Swim
      level: 45
    - skill: Spears
      level: 45
    - skill: Pickaxes
      level: 45
    - skill: WoodCutting
      level: 45
    - skill: Knives
      level: 45
    - skill: Polearms
      level: 45
    - skill: Clubs
      level: 45
    - skill: Unarmed
      level: 45
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: Mistlands
  items:
    - item: SwordBlackmetal
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 1
    - item: ShieldBlackmetal
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 2
    - item: ShieldBlackmetalTower
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 2
    - item: Hammer
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Hoe
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: BowDraugrFang
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 3
    - item: ArmorPaddedCuirass
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorPaddedGreaves
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetPadded
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeWolf
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: LoxPie
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: SerpentStew
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: Bread
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: BloodPudding
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: FishWraps
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: WolfJerky
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: ArrowSilver
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: ArrowNeedle
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: ArrowFrost
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: ArrowPoison
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: ArrowFire
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: BattleaxeCrystal
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 4
    - item: PickaxeIron
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 6
    - item: SpearWolfFang
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 5
    - item: Torch
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: AtgeirBlackmetal
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 7
    - item: MaceNeedle
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 7
    - item: KnifeBlackMetal
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 7
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: 1
  items:
    - item: SledgeDemolisher
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 1
    - item: ShieldBlackmetal
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 2
    - item: CrossbowArbalest
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: Hammer
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 8
    - item: Hoe
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 0
    - item: BowSpineSnap
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: ArmorCarapaceChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorCarapaceLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetCarapace
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeFeather
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: LoxPie
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: Demister
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: Bread
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: SeekerAspic
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: YggdrasilPorridge
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: MushroomOmelette
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: StaffFireball
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: ArrowNeedle
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: StaffIceShards
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 8
    - item: ArrowPoison
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: BoltBlackmetal
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: SwordMistwalker
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: PickaxeIron
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 6
    - item: THSwordKrom
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: Torch
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: AtgeirHimminAfl
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: KnifeSkollAndHati
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: BoltBone
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 7
    - item: BoltIron
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 7
    - item: HelmetMage
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: ArmorMageChest
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: ArmorMageLegs
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: ArmorMageLegs
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: ArmorMageLegs
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: MistlandsMage
  items:
    - item: StaffFireball
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 1
    - item: StaffIceShards
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: StaffShield
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: StaffSkeleton
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: SwordMistwalker
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: KnifeSkollAndHati
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: ArmorMageChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorMageLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetMage
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeFeather
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: YggdrasilPorridge
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MushroomOmelette
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: SeekerAspic
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: Salad
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: Demister
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: MisthareSupreme
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: Fisherman
  items:
    - item: FishingRod
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 1
    - item: Fish1
      quality: 1
      stack: 5
      use: true
      hotbarSlot: 3
    - item: Fish2
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 4
    - item: Fish3
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 5
    - item: Fish4_cave
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 6
    - item: Fish5
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 7
    - item: Fish6
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 8
    - item: Fish7
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 0
    - item: Fish8
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 0
    - item: Fish9
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 0
    - item: Fish10
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 0
    - item: Fish11
      quality: 1
      stack: 5
      use: false
      hotbarSlot: 0
    - item: Hammer
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: HelmetFenring
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorFenringLegs
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorFenringChest
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: FishingBait
      quality: 1
      stack: 50
      use: true
      hotbarSlot: 0
    - item: FishingBaitForest
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 0
    - item: FishingBaitSwamp
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 0
    - item: FishingBaitOcean
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 0
    - item: FishingBaitPlains
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 0
    - item: FishingBaitMistlands
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 4
    - item: FishingBaitDeepNorth
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 6
    - item: FishingBaitAshlands
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 0
    - item: FishCooked
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: Bread
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: BloodPudding
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: Fish1
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 0
    - item: Fish1
      quality: 5
      stack: 1
      use: false
      hotbarSlot: 0
    - item: Fish2
      quality: 2
      stack: 1
      use: false
      hotbarSlot: 0
    - item: Fish5
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 0
  skills:
    - skill: All
      level: 40
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: MistlandsWarrior
  items:
    - item: SwordMistwalker
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 1
    - item: THSwordKrom
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: SpearCarapace
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: SledgeDemolisher
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: KnifeSkollAndHati
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: AxeJotunBane
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: AtgeirHimminAfl
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: ShieldCarapace
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 8
    - item: ShieldCarapaceBuckler
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: BowSpineSnap
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: CrossbowArbalest
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: ArrowCarapace
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: BoltCarapace
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 0
    - item: PickaxeBlackMetal
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: Hammer
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: Hoe
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: HelmetCarapace
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorCarapaceChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorCarapaceLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeFeather
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: DvergrKey
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: Demister
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: MisthareSupreme
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeatPlatter
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: Salad
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadHealthMajor
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadStaminaLingering
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadPoisonResist
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: Ashlands
  items:
    - item: AxeBerzerkr
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 1
    - item: SwordNiedhogg
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: THSwordSlayer
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: MaceEldner
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: ArmorFlametalChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 5
    - item: ArmorFlametalLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 6
    - item: HelmetFlametal
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 7
    - item: MeatPlatter
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 1
    - item: Salad
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 1
    - item: HoneyGlazedChicken
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 1
    - item: MeadStaminaLingering
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 1
    - item: MeadHealthMajor
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 1
    - item: ArmorMageChest_Ashlands
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: ArmorMageLegs_Ashlands
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: HelmetMage_Ashlands
      quality: 1
      stack: 1
      use: true
      hotbarSlot: -1
    - item: SwordNiedhoggBlood
      quality: 1
      stack: 1
      use: true
      hotbarSlot: -1
    - item: SwordNiedhoggLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: SwordNiedhoggNature
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: MaceEldnerBlood
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: MaceEldnerLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: MaceEldnerNature
      quality: 1
      stack: 1
      use: true
      hotbarSlot: -1
    - item: THSwordSlayerBlood
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: THSwordSlayerLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: THSwordSlayerNature
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: AxeBerzerkrBlood
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: AxeBerzerkrLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: AxeBerzerkrNature
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: AshlandsFood
  items:
    - item: Resin
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: Fiddleheadfern
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: SpicyMarmalade
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: ScorchingMedley
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: RoastedCrustPieUncooked
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: RoastedCrustPie
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: AsksvinMeat
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: CookedAsksvinMeat
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: VoltureMeat
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: CookedVoltureMeat
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: BoneMawSerpentMeat
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: CookedBoneMawSerpentMeat
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: FierySvinstew
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: MashedMeat
      quality: 0
      stack: 10
      use: false
      hotbarSlot: 0
    - item: PiquantPie
      quality: 1
      stack: 10
      use: false
      hotbarSlot: -1
    - item: PiquantPieUncooked
      quality: 1
      stack: 10
      use: false
      hotbarSlot: -1
    - item: SizzlingBerryBroth
      quality: 1
      stack: 10
      use: false
      hotbarSlot: -1
    - item: SparklingShroomshake
      quality: 1
      stack: 10
      use: false
      hotbarSlot: -1
    - item: Resin
      quality: 1
      stack: 10
      use: false
      hotbarSlot: -1
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: AshlandsMage
  items:
    - item: HelmetMage_Ashlands
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorMageChest_Ashlands
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorMageLegs_Ashlands
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: StaffFireball
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 1
    - item: StaffIceShards
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: StaffShield
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: StaffSkeleton
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: StaffClusterbomb
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: Resin
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: StaffLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: StaffGreenRoots
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 8
    - item: StaffRedTroll
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: SizzlingBerryBroth
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: SparklingShroomshake
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: RoastedCrustPie
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadEitrLingering
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadStaminaLingering
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadHealthMajor
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: BombLava
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 0
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: AshlandsMedium
  items:
    - item: HelmetAshlandsMediumHood
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorAshlandsMediumChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorAshlandsMediumlegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeAsksvin
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CrossbowRipper
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 1
    - item: CrossbowRipperLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: BowAshlands
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: MaceEldnerNature
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: AxeBerzerkrBlood
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: RoastedCrustPie
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: ScorchingMedley
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: PiquantPie
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadStaminaLingering
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadHealthMajor
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: BombLava
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 0
    - item: BoltCharred
      quality: 1
      stack: 200
      use: false
      hotbarSlot: 0
    - item: ArrowCharred
      quality: 1
      stack: 200
      use: false
      hotbarSlot: 0
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: AshlandsWarrior
  items:
    - item: SwordNiedhoggBlood
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 1
    - item: ShieldFlametal
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 2
    - item: ShieldFlametalTower
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 3
    - item: BowAshlands
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: THSwordSlayerLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: Hammer
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 6
    - item: CrossbowRipper
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: BoltCharred
      quality: 0
      stack: 200
      use: false
      hotbarSlot: 0
    - item: ArrowCharred
      quality: 0
      stack: 200
      use: false
      hotbarSlot: 0
    - item: MeadStaminaLingering
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadHealthMajor
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: PiquantPie
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MashedMeat
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: RoastedCrustPie
      quality: 0
      stack: 10
      use: true
      hotbarSlot: 0
    - item: PickaxeBlackMetal
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: AxeBerzerkrNature
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 8
    - item: ArmorFlametalChest
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorFlametalLegs
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: HelmetFlametal
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: CapeAsh
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: BombLava
      quality: 1
      stack: 50
      use: false
      hotbarSlot: 0
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: AshlandsWeapons
  items:
    - item: SwordNiedhogg
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: SwordNiedhoggBlood
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
    - item: SwordNiedhoggLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: SwordNiedhoggNature
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: THSwordSlayer
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: THSwordSlayerBlood
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: THSwordSlayerLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: THSwordSlayerNature
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: MaceEldner
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: MaceEldnerBlood
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: MaceEldnerLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: MaceEldnerNature
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: AxeBerzerkr
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: AxeBerzerkrBlood
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: AxeBerzerkrLightning
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: AxeBerzerkrNature
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 0
    - item: SpearSplitner
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
    - item: BowAshlands
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 7
  skills:
    - skill: All
      level: 30
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: Base
  items:
    - item: Hammer
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 1
    - item: Hoe
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 2
    - item: Torch
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 3
    - item: HelmetDverger
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 4
    - item: CryptKey
      quality: 1
      stack: 1
      use: false
      hotbarSlot: -1
    - item: Demister
      quality: 1
      stack: 1
      use: false
      hotbarSlot: 5
    - item: FishAndBread
      quality: 1
      stack: 10
      use: true
      hotbarSlot: -1
    - item: HoneyGlazedChickenUncooked
      quality: 1
      stack: 10
      use: true
      hotbarSlot: -1
    - item: YggdrasilPorridge
      quality: 1
      stack: 10
      use: true
      hotbarSlot: -1
    - item: SeekerAspic
      quality: 1
      stack: 10
      use: true
      hotbarSlot: -1
    - item: MagicallyStuffedShroom
      quality: 1
      stack: 10
      use: true
      hotbarSlot: -1
    - item: MushroomOmelette
      quality: 1
      stack: 10
      use: true
      hotbarSlot: -1
    - item: Salad
      quality: 1
      stack: 10
      use: true
      hotbarSlot: -1
    - item: MisthareSupreme
      quality: 1
      stack: 10
      use: true
      hotbarSlot: -1
  skills:
    - skill: All
      level: 70
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: Builder
  items:
    - item: Hammer
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 1
    - item: Hoe
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 2
    - item: Cultivator
      quality: 3
      stack: 1
      use: false
      hotbarSlot: 3
    - item: PickaxeBlackMetal
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 4
    - item: AxeJotunBane
      quality: 4
      stack: 1
      use: false
      hotbarSlot: 5
    - item: FishAndBread
      quality: 1
      stack: 10
      use: false
      hotbarSlot: 0
    - item: RoastedCrustPie
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: ScorchingMedley
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: SpicyMarmalade
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: MeadStaminaLingering
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
    - item: HelmetFenring
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorFenringChest
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: ArmorFenringLegs
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: BeltStrength
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 0
  skills:
    - skill: All
      level: 70
  dropCurrent: true
  price: 999
  costPrefab: Coins
  setEffect: Potion_eitr_minor
  setEffectAsGP: false
- name: "Give Axe"
  items:
    - item: "AxeBlackMetal"
      quality: 2
      stack: 1
      use: false
      hotbarSlot: 2
  dropCurrent: false
  price: 15
  costPrefab: "Iron"

- name: "Hunter Pack"
  items:
    - item: "BowHuntsman"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "ArrowSilver"
      quality: 1
      stack: 100
      use: false
      hotbarSlot: 2
    - item: "ArmorTrollLeatherChest"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "ArmorTrollLeatherLegs"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
  skills:
    - skill: "Bows"
      level: 50
  dropCurrent: false
  price: 75
  costPrefab: "TrollHide"
  requiredGlobalKey: "defeated_bonemass"

- name: "Miner's Delight"
  items:
    - item: "PickaxeIron"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "MeadStaminaMedium"
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 2
    - item: "ArmorIronChest"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "ArmorIronLegs"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
  skills:
    - skill: "Pickaxes"
      level: 60
  dropCurrent: false
  price: 120
  costPrefab: "Iron"

- name: "Wizard's Kit"
  items:
    - item: "StaffLightning"
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "ArmorFenringChest"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "ArmorFenringLegs"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "HelmetFenring"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "YggdrasilPorridge"
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
  skills:
    - skill: "All"
      level: 80
  dropCurrent: true
  price: 150
  costPrefab: "Etir"
  setEffect: "Potion_eitr_minor"
  setEffectAsGP: true

- name: "Warrior's Arsenal"
  items:
    - item: "SwordIron"
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "ShieldBanded"
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 2
    - item: "ArmorIronChest"
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "ArmorIronLegs"
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "HelmetIron"
      quality: 4
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "MeadHealthMajor"
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 0
  skills:
    - skill: "Swords"
      level: 100
    - skill: "Blocking"
      level: 100
  dropCurrent: true
  price: 200
  costPrefab: "Iron"

- name: "Builder's Pack"
  items:
    - item: "Hammer"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "Hoe"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 2
    - item: "Cultivator"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 3
    - item: "AxeIron"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 4
    - item: "Wood"
      quality: 1
      stack: 200
      use: false
      hotbarSlot: 0
  skills:
    - skill: "WoodCutting"
      level: 50
  dropCurrent: false
  price: 50
  costPrefab: "FineWood"

- name: "Explorer's Pack"
  items:
    - item: "Torch"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "CapeLox"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 2
    - item: "ArmorLeatherChest"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "ArmorLeatherLegs"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 0
    - item: "CookedDeerMeat"
      quality: 1
      stack: 20
      use: true
      hotbarSlot: 0
  skills:
    - skill: "Run"
      level: 70
    - skill: "Jump"
      level: 70
  dropCurrent: false
  price: 100
  costPrefab: "Silver"
  setEffect: "Potion_eitr_minor"
  setEffectAsGP: false
- name: "Chef's Delight"
  items:
    - item: "CookedDeerMeat"
      quality: 1
      stack: 20
      use: true
      hotbarSlot: 2
    - item: "CookedLoxMeat"
      quality: 1
      stack: 20
      use: true
      hotbarSlot: 3
    - item: "TurnipStew"
      quality: 1
      stack: 20
      use: true
      hotbarSlot: 4
    - item: "QueensJam"
      quality: 1
      stack: 20
      use: true
      hotbarSlot: 5
    - item: "MeadTasty"
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 6
  dropCurrent: false
  price: 60
  costPrefab: "Honey"

- name: "Blacksmith's Kit"
  items:
    - item: "Hammer"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 3
    - item: "PickaxeBronze"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 4
  skills:
    - skill: "Pickaxes"
      level: 40
  dropCurrent: false
  price: 100
  costPrefab: "Iron"

- name: "Seafarer's Bundle"
  items:
    - item: "ArmorPaddedCuirass"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 2
    - item: "ArmorPaddedGreaves"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 3
    - item: "HelmetIron"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 4
    - item: "MeadFrostResist"
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 5
  skills:
    - skill: "Swim"
      level: 50
  dropCurrent: false
  price: 80
  costPrefab: "FineWood"

- name: "Rogue's Pack"
  items:
    - item: "KnifeBlackMetal"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "KnifeSilver"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 2
    - item: "ArmorPaddedCuirass"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 3
    - item: "ArmorPaddedGreaves"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 4
    - item: "CapeTrollHide"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 5
    - item: "MeadHealthMinor"
      quality: 1
      stack: 5
      use: true
      hotbarSlot: 6
  skills:
    - skill: "Knives"
      level: 60
  dropCurrent: true
  price: 90
  costPrefab: "Silver"

- name: "Archer's Bundle"
  items:
    - item: "BowFineWood"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "ArrowFire"
      quality: 1
      stack: 100
      use: true
      hotbarSlot: 2
    - item: "ArrowFrost"
      quality: 1
      stack: 100
      use: true
      hotbarSlot: 3
    - item: "ArmorLeatherChest"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 4
    - item: "ArmorLeatherLegs"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 5
  skills:
    - skill: "Bows"
      level: 75
  dropCurrent: false
  price: 85
  costPrefab: "FineWood"

- name: "Necromancer's Pack"
  items:
    - item: "StaffSkeleton"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "ArmorFenringChest"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 2
    - item: "ArmorFenringLegs"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 3
    - item: "HelmetFenring"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 4
    - item: "MeadEitrLingering"
      quality: 1
      stack: 10
      use: true
      hotbarSlot: 5
  skills:
    - skill: "All"
      level: 65
  dropCurrent: true
  price: 100
  costPrefab: "Etir"

- name: "Guardian's Gear"
  items:
    - item: "ShieldIronTower"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "MaceIron"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 2
    - item: "ArmorIronChest"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 3
    - item: "ArmorIronLegs"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 4
    - item: "HelmetIron"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 5
    - item: "MeadHealthMedium"
      quality: 1
      stack: 5
      use: true
      hotbarSlot: 6
  skills:
    - skill: "Blocking"
      level: 70
    - skill: "Clubs"
      level: 70
  dropCurrent: true
  price: 110
  costPrefab: "Iron"

- name: "Pathfinder Pack"
  items:
    - item: "Torch"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "CapeLox"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 2
    - item: "ArmorLeatherChest"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 3
    - item: "ArmorLeatherLegs"
      quality: 3
      stack: 1
      use: true
      hotbarSlot: 4
    - item: "MeadHealthMinor"
      quality: 1
      stack: 5
      use: true
      hotbarSlot: 5
  skills:
    - skill: "Run"
      level: 60
    - skill: "Sneak"
      level: 60
  dropCurrent: false
  price: 75
  costPrefab: "Silver"

- name: "Fisherman's Friend"
  items:
    - item: "FishingRod"
      quality: 1
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "FishingBait"
      quality: 1
      stack: 50
      use: true
      hotbarSlot: 2
    - item: "ArmorLeatherChest"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 3
    - item: "ArmorLeatherLegs"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 4
  skills:
    - skill: "Fishing"
      level: 50
  dropCurrent: false
  price: 60
  costPrefab: "Silver"

- name: "Defender's Pack"
  items:
    - item: "ShieldWood"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 1
    - item: "Club"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 2
    - item: "ArmorLeatherChest"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 3
    - item: "ArmorLeatherLegs"
      quality: 2
      stack: 1
      use: true
      hotbarSlot: 4
    - item: "MeadHealthMinor"
      quality: 1
      stack: 5
      use: true
      hotbarSlot: 5
  skills:
    - skill: "Blocking"
      level: 50
    - skill: "Clubs"
      level: 50
  dropCurrent: false
  price: 70
  costPrefab: "Bronze"


```

</details>

<details><summary><b>Images</b></summary>
![](https://i.imgur.com/DVlScer.png)

![](https://i.imgur.com/esB6C22.png)

![](https://i.imgur.com/EKznpar.png)

![](https://i.imgur.com/qunQ4Vt.png)

![](https://i.imgur.com/U2N4qjx.png)

![](https://media0.giphy.com/media/v1.Y2lkPTc5MGI3NjExNXMzMWg3MmlwbmFpbW5mcnU1YWc4bHFvcXkxbXhnYXp2NTgxZHlubCZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/4d61ywf3zzCap6m4kd/giphy.gif)

</details>

<br>
<br>

`Feel free to reach out to me on discord if you need manual download assistance.`

# Author Information

### Azumatt

`DISCORD:` Azumatt#2625

`STEAM:` https://steamcommunity.com/id/azumatt/

For Questions or Comments, find me in the Odin Plus Team Discord or in mine:

[![https://i.imgur.com/XXP6HCU.png](https://i.imgur.com/XXP6HCU.png)](https://discord.gg/qhr2dWNEYq)
<a href="https://discord.gg/pdHgy6Bsng"><img src="https://i.imgur.com/Xlcbmm9.png" href="https://discord.gg/pdHgy6Bsng" width="175" height="175"></a>