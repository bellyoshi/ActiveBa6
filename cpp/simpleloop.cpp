#include "./util.h"

#include <iostream>
using namespace std;

int main(int argc, char *argv[]){
    string s;
    while(true)
    {
        cout << "->";
        cin >> s;
        if(compIgnCase(s, "quit")){
            break;
        }
        cout << s << endl;
    }
    return 0;
}
