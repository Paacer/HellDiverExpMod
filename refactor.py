import os  
filepath = r'D:\CodeStore\GIT\UnityProject\HellDiverExpMod\Assets\Scripts\Components\CommandHolder.cs'  
with open(filepath, 'r', encoding='utf-8') as f:  
    content = f.read()  
start_idx = content.find('private void CmdMatch(char inputChar)')  
end_idx = content.find('private static bool EndsWithCommandPrefix')  
brace_idx = content.find('{', start_idx)  
brace_count = 0  
idx = brace_idx  
while idx < len(content):  
    if content[idx] == '{': brace_count += 1  
    elif content[idx] == '}': brace_count -= 1  
        if brace_count == 0: break  
    idx += 1  
method_end = idx 
