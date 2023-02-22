PROGRAM	= simpleloop
CPP	= g++
OBJ	= util.o simpleloop.o
run : $(PROGRAM)
	./$(PROGRAM)
$(PROGRAM) : $(OBJ)
	$(CPP) -o $(PROGRAM) $(OBJ)
util.o : util.cpp
	$(CPP) -c -o util.o util.cpp
simpleloop.o : simpleloop.cpp
	$(CPP) -c -o simpleloop.o simpleloop.cpp
clean:;
	rm -f *.o *~ $(PROGRAM)
