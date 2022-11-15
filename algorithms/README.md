# 4025C_VR, Algorithms, Dataformats, etc.
(V4 2022-11-14, by Peter Kienle)

1. Connection Node Network

 
## 1. **Connection Node Network**

3d assets, such as Iot Stars, Amaria and Amatria items, are imported into Unity in .fbx files. In order to be used for assembly, they are outfitted with connector nodes. These nodes are children of the item (parent). An item can have any number of nodes. 

In order to operate correctly, nodes in a parent item must be named in a specific way. The first node in every item is named "c0". Subsequent nodes are named "c1", "c2", etc.


### **A. Dataformats**

In addition to Unity-specific components (Mesh Renderer, Colliders, Shaders), the different object types have added components depending on their function.

#### **Manifests**

A manifest is the root game object for any connection node based assembly. Manifests reside in the top-level hierarchy. Each manifest has a Manifest Status script component where information about all the nodes in the manifest are recorded in a C# list called ConList. The second item is the ParentList, which contains all the parent items used in the manifest.

The application initially contains two manifests:

**Library**

The library manifest contains all items that can be used to build assemblies. Every time the user selects an item from the library to be connected to the growing assembly, a copy of that item is cloned from the library.

At the user level the library cannot be changed. ConList and ParentList for this manifest are automatically initialized on launch and don't change during operation.

**Manifest**

This is the root node where an assembly starts. It contains a single item with one node as a child. While the user is editing in the Assembly Area this manifest is the active assembly. Items from the library can be added on, items can be deleted from the assembly.

The building session ends when the user teleports back from Assembly to the Main Area. The manifest will be "packaged" and scaled to a tenth of its size. When the user arrives in the Main Area the object will be on one of the work tables in its "natural" size.

**Manifest List**

Every manifest that is packaged and transported to the Main Area gets an entry in the manifest list.

At the moment this list has no other functionality.

#### Items (Parents)

Items are gameobjects that reside inside a manifest. These items are the visible objects such has IoT Stars, etc. 

Like a manifest, each item has an added script component in the form of a Parent Data script. At the moment this script holds the parent type of this item. i.e., if we look at an item named "amaria_b1" this will also be the parent type string. When an item is cloned from the library Unity will add (1) or (clone 1) to the game object name. The parent type string however will stay the same. This ensures the item knows from which library objects it was cloned.

#### Connector Nodes

Connector nodes are small, clickable, spheres. An item must have at least (1) node. Nodes are children gameobjects of an item. They are the entities the user interacts with by clicking.

Nodes have color codes:

- green = can be clicked
- blue = when pointer hovers over it
- yellow = node is selected (only one node can be selected at a given time)
- red = this node is a connection in an assembly; clicking on it severs the connection and deletes everything attached to that node

All nodes contained in a specific manifest enter themselves into the ConList when the are created (Awake()). Nodes have these additional components:

*XR Simple Interactable* tracks when the user clicks this object
*ConStatus script* stores the status of this node. This includes Booleans for *show*, *selected*, *connected* and more. These flags are evaluated by ConController.cs.

During the assembly process only eligible nodes are visible. i.e. the default state will show one green node on every library item. As soon as the user selects one of these nodes, it will turn yellow (selected), while all the other items show their possible connection targets in green. The user then selects a target to finalize the connection, or clicks the yellow node again to abort the operation.

### B. Assembly Encoding for Saving

### C. Assembly Decoding for Loading

