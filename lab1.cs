using System;

class Stack64U
{

    private ulong[] stack, min_stack;
    private int top, min_top;
    private int size = 1;

    public Stack64U(){    // Constructor
        stack = new ulong[size];
        min_stack = new ulong[size]
        top = -1;   // Stack starts with no elements
        min_top = -1;
    }

    public void Push(ulong data){
        if (top + 1 > size)     // Size handling
        {
            Array.Resize(ref stack, size*2);
            size = size / 2;
        }
        if(top == -1){ 
            min_stack[++min_top] = data;
        }
        if(data <= min_stack[min_top] )
        {
            min_stack[++min_top] = data;
        }
        stack[++top = data];
    }
    public Pop(){
        if (top == -1)
        {
            throw new Exception("Stack is empty (pop)");
        }
        if (stack[top] == min_stack[min_top]){
            min_top--;
        }

        top--;
        if (top + 1 == size / 2){   // Size handling
            Array.Resize(ref stack, size / 2);
            size = size / 2;
        }
        return top;

    }
    public Peek(){
        if (top == -1)
        {
            throw new Exception("Stack is empty (peek)");
        }
        return stack[top];
    }
    public StackSize(){
        return top + 1;
    }
    public IsEmpty(){
        if(top == -1){
            return true;
        }
        else{
            return false;
        }
    }
    public GetMinVal(){
        return min_stack[min_top];
    }
    public void PrintStack()
    {
        for (int i = 0; i < top + 1; i++)
        {
            Console.WriteLine(stack[i]);
        }
    }
}

class program
{
    static void Main(){
        Stack64U stack = new Stack64U();
        stack.Push(6);
        stack.Push(5);
        stack.Push(10);
        stack.Push(4);
        stack.Push(3);
        stack.Push(2);

        stack.Push(20);
        stack.Push(8);
        stack.Push(9);
        stack.Push(10);
        stack.Push(11);
        stack.Push(12);
        stack.Push(1);
        stack.Push(14);
        stack.Push(15);
        stack.Push(16);
        stack.Push(22);
        stack.Push(1);
        stack.PrintStack();
}

}
