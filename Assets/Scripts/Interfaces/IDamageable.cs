using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable //An interface that houses the method "damage"
{
    public void Damage(float damage);//A method that is meant to be used whenever the user wants to damage something, either themselves or another object
}
