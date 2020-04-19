
# Building assembly for Visual Studio

 * Download VirtualBox SDK from https://www.virtualbox.org/
 * Extract `sdk/bindings/mscom/lib/VirtualBox.tlb`
 * Run `tlbimp /namespace:VirtualBox61 /out:VirtualBox61.dll VirtualBox.tlb` to obtain `VirtualBox61.dll`
