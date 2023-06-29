# Description

## A mod that adds ItemSets to the player's inventory. Pre-configured sets that can be purchased. It's highly configurable. Use your imagination!

`Version checks with itself. If installed on the server, it will kick clients who do not have it installed.`

`This mod uses ServerSync, if installed on the server and all clients, it will sync all configs to client`

`This mod uses a file watcher. If the configuration file is not changed with BepInEx Configuration manager, but changed in the file directly on the server, upon file save, it will sync the changes to all clients.`


---

Anything you do not find in this document will be added to the Wiki Tab for this mod. The wiki is the only thing I can
live update if needed. It will also contain the information found on this page, just in case. If you have any questions,
please feel free to ask *AFTER* reading the documentation.

"But Azu, where is the documentation/wiki?" - You, probably

[![](https://i.imgur.com/yKjqxpC.png)](https://valheim.thunderstore.io/package/Azumatt/RapidLoadouts/wiki/)

The ItemSets button and window are both draggable. You can move them around the screen to your liking. The window/button
will remember their positions between sessions. CTRL + RightClick to drag.

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

The YAML file (`Azumatt.RapidLoadouts_ItemSets.yml`) that is created by the mod allows for configuration of ItemSets.
The entries specify ItemSets (in game items, skills, effects) that can be obtained in the game for a specific price.

Each ItemSet is represented by a YAML entry. The entry contains information about the items, skills, or effects that
will be given to the player upon purchase, along with their respective properties such as quantity, quality, and price.

## General Structure of the YAML

- `name`: This is the name of the ItemSet that will appear in-game. Make sure this is unique.

- `items`: This is a list of items included in the ItemSet. Each item has several properties:
    - `item`: The prefab name of the item.
    - `quality`: The quality level of the item.
    - `stack`: The quantity of this item in the ItemSet.
    - `use`: If true, the item will be automatically used (consumed/used/equipped) upon purchase.
    - `hotbarSlot`: The position in the player's hotbar where the item will be placed. If multiple items have the same
      slot, they will be added in the order they appear while swapping each other out in the hotbar. The last item added
      will be the one that wins the slot.

- `skills`: This is a list of skills included in the ItemSet. Each skill has several
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

- `price`: The cost of the ItemSet in-game.

- `costPrefab`: This specifies an item that will be used as an alternative form of payment. If present, the ItemSet can
  be bought with the specified item instead of the default currency found in the configuration file (
  Section: `1 - General` Configuration: `ItemSetCostPrefab`).

## ItemSets

The file contains several preconfigured ItemSets such as:

1. "Give Hammer" – gives a hammer of quality 3, for the price of 10 Fine Wood.

2. "AzuMage" – provides a set of armor, a staff, and food items, sets all skills to level 100, and costs 60 Etir. It
   also drops the player's current items.

3. "Potion Pack" – includes minor and medium health meads, costing 50 of the default currency.

4. "Testing Pack" – grants a bronze sword and sets the skill levels of 'Swords' to 16 and 'Run' to 100, for the cost of
   100 Bronze.

5. Several ItemSets provide status effects such as "Status Effect Test", "Goblin Shaman Shield", "Rested Buff level 1",
   and "Corpse Run Buff", with varying costs.

6. There are also ItemSets that boost the player's skill levels: "+10 To Skills", "+20 To Skills", and "Max Out Skills".
   Each ItemSet boosts all skills by a certain amount and has its own cost.

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
- name: "Testing Pack"
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
  price: 100
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

```

</details>

<details><summary><b>Images</b></summary>

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

[![https://i.imgur.com/XXP6HCU.png](https://i.imgur.com/XXP6HCU.png)](https://discord.gg/Pb6bVMnFb2)
<a href="https://discord.gg/pdHgy6Bsng"><img src="https://i.imgur.com/Xlcbmm9.png" href="https://discord.gg/pdHgy6Bsng" width="175" height="175"></a>