# FREE STYLIZED URP SHADERS - SETUP & USAGE GUIDE

Thank you for downloading this shader pack!

These shaders are specifically written for Unity's **Universal Render Pipeline (URP)**. If the materials appear **PINK (Magenta)** when you import the package, it means URP is either not installed, not active, or not configured correctly in your project.

To ensure everything works flawlessly, please follow the steps below to configure the included URP settings in your project.

---

## STEP 1: Ensure URP is Installed
*If your project is already a URP project, you can skip this step.*

1. Open **Window > Package Manager** from the top menu.
2. Click the dropdown next to "Packages:" in the top left and select **Unity Registry**.
3. Type **Universal RP** in the search bar.
4. Make sure it is installed. If not, click **Install**.

---

## STEP 2: Configure the Pipeline Asset (Fix Pink Materials)
You need to assign the optimized URP configuration file included in this package to your project's graphics settings:

1. Open **Edit > Project Settings** from the top menu.
2. Select the **Graphics** tab from the left-hand menu.
3. Locate the **Scriptable Render Pipeline Settings** field at the very top.
4. Click the small circle icon next to the empty field (or drag-and-drop).
5. Choose **URP_Pipeline Asset** (which is located inside the *Pipelines* folder of this package).

*(Note: It is highly recommended to also go to **Project Settings > Quality** and assign the same file to the **Render Pipeline Asset** slot there as well.)*

Once assigned, Unity will automatically recompile the shaders, and the pink color will transform into your beautiful stylized visual effects!

---

## FOLDER STRUCTURE
- 📂 **Demo** – An example scene showcasing the shaders in action.
- 📂 **Material** – Pre-configured sample materials ready to use.
- 📂 **Pipelines** – Ready-to-use URP Asset configuration files for your project.
- 📂 **Shaders** – Highly optimized Lit and Unlit HLSL shader codes.
- 📂 **Textures** – Noise maps and main textures used by the effects.

---

## REQUIREMENTS
- **Unity Version:** Unity 2022.3 or newer
- **Render Pipeline:** Universal Render Pipeline (URP)

Happy game developing!