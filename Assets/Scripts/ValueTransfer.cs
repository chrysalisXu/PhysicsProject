using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ValueTransfer : MonoBehaviour
{
    public string theValue;
    public GameObject inputFieldSpring;
    public GameObject textDisplaySpring;
    public GameObject inputFieldPlastic;
    public GameObject textDisplayPlastic;
    public GameObject inputFieldDampingForce;
    public GameObject textDisplayDampingForce;

    public ClothPhysics.Cloth clothObj;
    public void StoreSpringValue()
    {
        theValue = inputFieldSpring.GetComponent<Text>().text;
        textDisplaySpring.GetComponent<Text>().text = theValue;

        clothObj.springCoefficient = float.Parse(theValue);
        print("Cloth springCoefficient: ");
        print(clothObj.springCoefficient);
    }

    public void StorePlasticityValue()
    {
        theValue = inputFieldPlastic.GetComponent<Text>().text;
        textDisplayPlastic.GetComponent<Text>().text = theValue;
        
        clothObj.plasticity = float.Parse(theValue);
        print("Cloth plasticity: ");
        print(clothObj.plasticity);
    }
    public void StoreDampingForceValue()
    {
        theValue = inputFieldDampingForce.GetComponent<Text>().text;
        textDisplayDampingForce.GetComponent<Text>().text = theValue;
        
        clothObj.dampingCoefficient = float.Parse(theValue);
        print("Cloth damping force Coefficient: ");
        print(clothObj.dampingCoefficient);
    }
}
