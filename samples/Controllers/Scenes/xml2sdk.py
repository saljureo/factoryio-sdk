# Import BeautifulSoup
from bs4 import BeautifulSoup as bs
content = []
events = ''
states = ''
stateNames = []
transitions = ''
eventLabel = ''
eventId = ''
AddString = ''
initialState = ''
noncontrollableEvents = []
factoryIoFileName = ''
XmlFileName = ''
csFileName = ''
# Read the XML file

factoryIoFileName = 'machines2AndBuffer'
XmlFileName = factoryIoFileName + '.xml'
#with open("supervisorBuffer2.xml", "r") as file:
with open(XmlFileName, "r") as file:
    # Read each line in the file, readlines() returns a list of lines
    content = file.readlines()
    # Combine the lines in the list into a string
    content = "".join(content)
    bs_content = bs(content, "lxml") 

events = bs_content.find_all("event")
states = bs_content.find_all("state")
transitions = bs_content.find_all("transition")

for state in states:
    nombre = state["name"]
    try: 
        state["initial"]
        initialState = state["id"]
    except:
        continue
    stateNames.append(nombre)

csFileName = factoryIoFileName + 'Supervisor.cs'
try:
    f = open(csFileName, "x")
except:
    f = open(csFileName, "w")

##############  PROGRAM WRITING STARTS  ########################
#Headers
f.write("using System;\n")
f.write("using System.Collections.Generic;\n")
f.write("using System.Threading;\n\n")

#namespace Controllers - class 
f.write("namespace Controllers.Scenes\n")
f.write("{\n")
stringy = "    class " + factoryIoFileName + "Supervisor\n"
f.write(stringy)
f.write("    {\n\n")
f.write("        // #### VARIABLE CREATION TO ALLOCATE IN MEMORY ####\n")
f.write("        private int currentState;\n")
f.write("        private int evento;\n")
f.write("        private Dictionary<(int, int), int> transiciones;\n")
f.write("        private Dictionary<string, int> eventLabels;\n\n")
f.write("        private Dictionary<int, string> stateLabels;\n\n")
f.write("        public void CreateController()\n")
f.write("        {\n")
f.write("            transiciones = new Dictionary<(int, int), int>();\n")
f.write("            eventLabels = new Dictionary<string, int>();\n")
f.write("            stateLabels = new Dictionary<int, string>();\n\n")

f.write("            currentState = " + initialState + ";\n") 
f.write("            //#########  TRANSICIONES START ############\n\n")


for transition in transitions:
    AddString ='            ' + 'transiciones.Add((' + transition["source"] + ', ' + transition["event"] + '), ' + transition["dest"] + ');\n'
    f.write(AddString)
f.write("\n            //#########  TRANSICIONES END ############\n\n")
f.write("            //#########  EVENTLABEL START ############\n\n")

for event in events:    
    AddString = '            ' + 'eventLabels.Add("' + event["label"] + '", ' + event["id"] + ');\n'
    f.write(AddString)
    print(event)
    try:
        event["controllable"]
        noncontrollableEvents.append(event["id"])
        print("evento no controlable: " + event["id"] + " con label: " + event["label"])
    except:
        continue
f.write("\n            //#########  EVENTLABEL END ############\n\n")
f.write("\n            //#########  STATELABEL START ############\n\n")
for state in states:
    AddString = '            ' + 'stateLabels.Add(' + state["id"] + ', "' + state["name"] + '");\n'
    f.write(AddString)
    print(state)

f.write("\n            //#########  STATELABEL END ############\n\n")
stringy = '            Console.WriteLine("' + r'\n' + 'Current state is: " + stateLabels[currentState] + ' + r'"\n"' + ');\n'
f.write(stringy)
f.write("        }\n\n")
f.write("        public bool On(string eventoLabel)\n")
f.write("        {\n")
f.write("            evento = eventLabels[eventoLabel];\n")
f.write("            if (transiciones.ContainsKey((currentState, evento)))\n")
f.write("            {\n")
f.write("                currentState = transiciones[(currentState, evento)];\n")

if bool(noncontrollableEvents):
    stringy = "                if ("
    for count,event in enumerate(noncontrollableEvents):
        stringy = stringy + "evento != " + event
        if count != len(noncontrollableEvents) - 1:            
            stringy = stringy + " && "
    stringy = stringy + ")\n"
    f.write(stringy)
    f.write('                {\n')
    f.write('                    Console.WriteLine(eventoLabel + " event approved");\n')
    f.write('                }\n')
    f.write('                else\n')
    f.write('                {\n')
    f.write('                    Console.WriteLine(eventoLabel + " event is uncontrollable and must be enabled");\n')
    f.write('                }\n')
else:
    f.write('                    Console.WriteLine(eventoLabel + " event approved");\n')



stringy = r'                Console.WriteLine("Current state is: " + stateLabels[currentState] + "\n");'
f.write(stringy + '\n')
f.write('                return true;\n')
f.write('            } else\n')
f.write('            {\n')
f.write('                Console.WriteLine(eventoLabel + " event blocked");\n')
f.write('                Thread.Sleep(800);\n')
f.write('                return false;\n')
f.write('            }\n')
f.write('        }\n')
f.write('    }\n')
f.write('}')
f.close()
    
for state in states:
    print(state)



    

##############  PROGRAM WRITING ENDS  ########################

"""
        public bool On(string evento)
        {
            if (diccionario.ContainsKey((currentState, evento)))
            {
                currentState = diccionario[(currentState, evento)];
                if (evento != "f1" && evento != "f2" && evento != "b1" && evento != "b2")
                {
                    Console.WriteLine(evento + " event approved");
                }
                else
                {
                    Console.WriteLine(evento + " event is uncontrollable and must be enabled");
                }
                Console.WriteLine("Current state is: " + currentState + "\n");
                return true;
            } else
            {
                Console.WriteLine(evento + " event blocked");
                Thread.Sleep(800);
                return false;
            }
        }
    }
}

"""


"""

#States and transitions - FIND INITIAL STATE
for state in states:
    try:        
        whatever = state["initial"]        
        initialStateName = state["name"]
        initialStateId = state["id"]
    except:
        pass

actualStateName = initialStateName
actualStateId = initialStateId

while(thereAreOtherStates == True): 
    f = open("myfile.cs","a")
    f.write('\nif (supervisorState == SupervisorState.'+ actualStateId+')\n')
    f.write('{\n')
    f.write('   //fill actions for state ' + actualStateName + "\n")
    f.close()

    transitionsFoundFromActualState = bs_content.find_all("transition", {"source":actualStateId})
    for transition in transitionsFoundFromActualState:
        transitioningEventId = transition["event"]
        eventNameFoundTemp = bs_content.find_all("event", {"id":transitioningEventId})
        eventNameFound = eventNameFoundTemp[0]["label"]
        nextStateId = transition["dest"]
        nextStateNameFoundTemp = bs_content.find_all("state", {"id":nextStateId})
        nextStateNameFound = nextStateNameFoundTemp[0]["name"]
        f = open("myfile.cs","a")
        f.write('   if (supervisorEvent == SupervisorEvent.'+ eventNameFound + ")//Which is event "+ transitioningEventId + "\n")
        f.write("   {\n")
        f.write("       supervisorState = SupervisorState." + nextStateNameFound + ";\n")
        f.write("   }\n")
        f.close()
        actualStateId = nextStateId
    f = open("myfile.cs","a")
    f.write('}')
    f.close()

#%%%%%%%%%%%%%%%%%% Public enum States %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
f = open("myfile.cs","a")
f.write('\npublic enum SupervisorState\n')
f.write('{\n')
longitud = len(states)
for i,state in enumerate(states):
    nombre = state["name"]
    f.write('   ' + nombre)
    if (i+1 != longitud):
        f.write(',\n')
f.write('\n}\n')
f.close()

#%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% Public enum Events %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
f = open("myfile.cs","a")
f.write('\npublic enum SupervisorEvent\n')
f.write('{\n')
longitud = len(events)
for i,event in enumerate(events):
    nombre = event["label"]
    f.write('   ' + nombre)
    if (i+1 != longitud):
        f.write(',\n')
f.write('\n}\n')
f.close()


#open and read the file after the appending:
f = open("myfile.cs", "r")
print(f.read()) 

"""

