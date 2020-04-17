
# Building assembly for Visual Studio

 * Download VirtualBox SDK from https://www.virtualbox.org/
 * Extract `sdk/bindings/mscom/lib/VirtualBox.tlb`
 * Run `tlbimp /namespace:VirtualBox616 /out:VirtualBox616.dll VirtualBox.tlb` to obtain `VirtualBox616.dll`
