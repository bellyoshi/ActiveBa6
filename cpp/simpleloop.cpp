#include "./util.h"

#include <iostream>
#include <stack>
using namespace std;

stack<double> machine_stack;
void printstack(stack<double> stack);
int getTop2Elm(stack<double> &s , double *a, double *b);


int main(int argc, char *argv[]){
    string s;
    while(true)
    {
        cout << "->";
        cin >> s;
        if(compIgnCase(s, "quit")){
            break;
        }
        if(compIgnCase(s, "printstack"))
        {
            printstack(machine_stack);
            continue;
        }
        char c = s[0];
        switch(c){
            case '+':{
                double a,b;
                if(getTop2Elm(machine_stack, &a, &b))
                    break;
                cout << a + b << endl;
                machine_stack.push(a + b);
                break;
            }
            case '-':{
                double a,b;
                if(getTop2Elm(machine_stack, &a, &b))
                    break;
                cout << a - b << endl;
                machine_stack.push(a - b);
                break;
            }
            case '*':{
                double a,b;
                if(getTop2Elm(machine_stack, &a, &b))
                    break;
                cout << a * b << endl;
                machine_stack.push(a * b);
                break;
            }

            case '/': {
                double a,b;
                if(getTop2Elm(machine_stack, &a, &b))
                    break;
                cout << a + b << endl;
                machine_stack.push(a / b);
            }          
                break;
            default:{
                machine_stack.push(atof(s.c_str()));
                break;
            }
                
        }
        
        cout << s << endl;
    }
    return 0;
}
void printstack(stack<double> display_stack)
{
    stack<double> tmpstack;
    tmpstack = display_stack;
    cout << "--- stack top ---" << endl;
    int n = (int)tmpstack.size();
    for(int i = 0; i < n ; i++){
        cout << tmpstack.top() << endl;
        tmpstack.pop();
    }
    cout << "--- stack buttom ---" << endl;
}

int getTop2Elm(stack<double> &s , double *a, double *b)
{
    if(s.size() < 2)
    {
        cout << "few operand" << endl;
        return -1;
    }
    *a = s.top();
    s.pop();
    *b = s.top();
    s.pop();
    return 0;
}


