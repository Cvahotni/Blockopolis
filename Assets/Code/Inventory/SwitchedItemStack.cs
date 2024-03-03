public struct SwitchedItemStack
{
    public ItemStack itemStack;
    public float switchTime;

    public SwitchedItemStack(ItemStack itemStack, float switchTime) {
        this.itemStack = itemStack;
        this.switchTime = switchTime;
    }
}
