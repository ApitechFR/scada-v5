#Apitech - Scada Scheme Editor
##Description
This project is forked from Rapid Scada Sceme Editor. It contains some additional functionalities :
* A treeview with every components of the scheme
* A Group/Ungroup button to create groups of components
* A new type of component : ```Symbol```. It is a reusable component the user can build by gathering non-symbol components.

##Components Treeview
##Group/Ungroup
##Symbols

Rapid SCADA
===========

Rapid SCADA is full featured SCADA software that supports MODBUS and OPC.

Website https://rapidscada.org

# Documentation
This documentation is intended for developers. It contains insctructions to correctly collaborate on the project and it is made to be updated as needed.

## Git repository
### Gitflow
In the repository, we use the gitflow workflow, in addition to a separated branch for production : ```master```, ```production``` and ```develop``` are the only branches that are permanent, and no one should commit changes to these three branches.

```master``` is linked to the community project, ```production``` is used to deploy solution to client, and ```develop``` to do the development.

* The process is to create one new branch from ```develop``` for every feature or bugfix, and then make a pull request to apply changes into ```master``` (or ```production```). 
* ```master```, as well as ```production``` should only be modified via pull request from ```develop``` or a hotfix branch.
* To perform a hotfix, create a new branch from ```master``` (or ```production```), and then make a pull request from this branch to ```master``` (or ```production```). After the hotfix, make sure to align ```develop``` with ```master``` (or ```production```) to avoid any conflict during next pull request from ```develop``` to ```master``` (or ```production```).

### Naming conventions
#### Branches
Every feature or fix branch is supposed to be removed after merge.
By convention, branches names have to :
* Be short (ideally one word)
* Be lowercase
* Be hyphen-separated (if more than one word)
* Start with ```hotfix-``` if it is a hotfix-linked branch

#### Commits
Commits comments have to respect this template : ```type(scope): subject``` or ```type: subject```.

##### Type
Type is on of these only :
* ```feat```: Add of new functionality
* ```fix```: bug fix
* ```build```: Change linked to build system or dependencies (npm, grunt, gulp, webpack, etc.)
* ```ci```: Change linked to CI/CD (Jenkins, Travis, Ansible, gitlabCI, etc.)
* ```docs```: Change linked to documentation (README, JSdoc, etc.)
* ```perf```: Performance improvement
* ```refactor```: Modification that doesn't add functionality or fix anything (variable renaming, redundant code remove, code simplify, etc.)
* ```style```: Changement lié au style du code (indentation, point virgule, etc.);
* ```test```: Tests add or modification;
* ```revert```:  Annulation of previous commit;
* ```chore```: Any other modification (version update for example).

##### Scope
Scope is facultative and simply describe the context of the commit in one word. It may be a project component. In our case, it can be for example ```moderation``` or ```chorum```.

##### Subject
Subject is a short description (ideally less thant 50 characters) of the modification.

### Best practices
To keep the code and repository as clean as possible, we follow these rules:

* Merge a pull request only when it was reviewed by another developer (Github allows to add a reviewer to pull requests)
* Don't commit change directly to ```master``` or ```develop```
* Respect naming conventions
  
