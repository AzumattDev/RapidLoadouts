using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace RapidLoadouts.YAMLStuff;

[Serializable]
public class ItemSet
{
    [YamlMember(Alias = "name")] public string m_name = null!;
    
    [YamlMember(Alias = "items")] public List<SetItem> m_items = new();

    [YamlMember(Alias = "skills")] public List<SetSkill> m_skills = new();

    [YamlMember(Alias = "dropCurrent")] public bool m_dropCurrent;
    
    [YamlMember(Alias = "price")] public int m_price;
    
    [YamlMember(Alias = "costPrefab")] public string m_prefabCost = null!;
    
    [YamlMember(Alias = "setEffect")] public string m_setEffect = null!;

    [YamlMember(Alias = "setEffectAsGP")] public bool m_setEffectAsGP;
}

[Serializable]
public class SetItem
{
    [YamlMember(Alias = "item")] public string m_item = null!;

    [YamlMember(Alias = "quality")] public int m_quality = 1;

    [YamlMember(Alias = "stack")] public int m_stack = 1;

    [YamlMember(Alias = "use")] public bool m_use = true;

    [YamlMember(Alias = "hotbarSlot")] public int m_hotbarSlot;
}

[Serializable]
public class SetSkill
{
    [YamlMember(Alias = "skill")] public string m_skill = null!;

    [YamlMember(Alias = "level")] public int m_level;
}