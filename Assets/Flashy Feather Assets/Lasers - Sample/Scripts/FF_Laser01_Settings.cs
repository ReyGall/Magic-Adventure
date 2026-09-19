using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ff_laser_animations_01{
public class FF_Laser01_Settings : MonoBehaviour
{
    public float laser_duration = 1.8f;
    public float length_multiplier = 1f;
    public bool DESTROY_ON_END = false;
    public Transform t_main_laser;
    public ParticleSystem[] ps_main;
    public ParticleSystem[] ps_muzzle;
    public ParticleSystem[] ps_anticipation;
    public ParticleSystem ps_small_particles;



    void Awake(){
        //-----LENGHT-----//
        if( length_multiplier <= 0.1f ){
            print("minimum length multiplier is 0.1");
            length_multiplier = 0.1f;
        }
        t_main_laser.localScale = new Vector3(t_main_laser.localScale.x * length_multiplier, t_main_laser.localScale.y, t_main_laser.localScale.z);

        //----DURATION----//   
        if( laser_duration < 0 ){
            print("duration can't be less than zero!");
            return;
        }
        for( int i = 0; i < ps_main.Length; i++ ){
            ps_main[ i ].Stop();
            var main = ps_main[ i ].main;
            main.duration = laser_duration + 1.2f;
            main.startLifetime = laser_duration;
            ps_main[ i ].Play();
        }
        for( int i = 0; i < ps_muzzle.Length; i++ ){
            ps_muzzle[ i ].Stop();
            var main_b = ps_muzzle[ i ].main;
            main_b.duration = laser_duration + 1.2f;
            int new_bursts = Mathf.RoundToInt( (laser_duration - 0.8f)*10f );

            var em_b = ps_muzzle[ i ].emission;
            em_b.enabled = true;
            em_b.rateOverTime = 0;
            em_b.SetBursts(
                new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(0.55f, 1, new_bursts, 0.1f),
                });

            ps_muzzle[ i ].Play();
        }
        ps_small_particles.Stop();
        var main_c = ps_small_particles.main;
        main_c.duration = laser_duration + 1.2f;
        int new_bursts_b = Mathf.RoundToInt( (laser_duration - 0.4f)*10f );
        
        var em = ps_small_particles.emission;
        em.enabled = true;
        em.rateOverTime = 0;
        em.SetBursts(
            new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0.0f, 22, new_bursts_b, 0.1f),
            });
        ps_small_particles.Play();

        for( int i = 0; i < ps_anticipation.Length; i++ ){
            ps_anticipation[ i ].Stop();
            var main = ps_anticipation[ i ].main;
            main.duration = laser_duration + 1.2f;
            ps_anticipation[ i ].Play();
        }
        
        //----DESTROY----//
        if( DESTROY_ON_END )
            Destroy( gameObject, laser_duration + 1.15f );     
    }


    
}
}

