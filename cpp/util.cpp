#include "./util.h"

bool compCharIgnCase(char c1, char c2){
    return (tolower(c1)== tolower(c2));
}
bool compIgnCase(string s1, string s2){
    if(s1.length() != s2.length()){
        return false;
    }
    return equal(s1.begin(), s1.end(),
                s2.begin(), compCharIgnCase);
}