import * as THREE from 'three';
import { STLLoader } from 'three/addons/loaders/STLLoader.js';
import { OrbitControls } from 'three/addons/controls/OrbitControls.js';

document.addEventListener('DOMContentLoaded', function () {
  document.querySelectorAll('.stl-viewer').forEach(function (container) {
    if (container.dataset.initialized) return;
    container.dataset.initialized = 'true';

    var url = container.dataset.stl;
    if (!url) return;

    var width = container.clientWidth || 600;
    var height = parseInt(container.dataset.height) || 400;
    var color = container.dataset.color || '#2194ce';
    var bgColor = container.dataset.bg || '#f5f5f5';

    var scene = new THREE.Scene();
    scene.background = new THREE.Color(bgColor);

    var camera = new THREE.PerspectiveCamera(50, width / height, 0.1, 2000);
    var renderer = new THREE.WebGLRenderer({ antialias: true });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(width, height);
    container.appendChild(renderer.domElement);

    scene.add(new THREE.AmbientLight(0x404040, 2));
    var d1 = new THREE.DirectionalLight(0xffffff, 1.5);
    d1.position.set(1, 1, 1);
    scene.add(d1);
    var d2 = new THREE.DirectionalLight(0xffffff, 0.5);
    d2.position.set(-1, -1, -0.5);
    scene.add(d2);

    var controls = new OrbitControls(camera, renderer.domElement);
    controls.enableDamping = true;
    controls.dampingFactor = 0.1;

    var loader = new STLLoader();
    loader.load(url, function (geometry) {
      var material = new THREE.MeshPhongMaterial({
        color: new THREE.Color(color),
        specular: 0x111111,
        shininess: 80
      });
      var mesh = new THREE.Mesh(geometry, material);

      geometry.computeBoundingBox();
      var center = new THREE.Vector3();
      geometry.boundingBox.getCenter(center);
      mesh.position.sub(center);

      var size = new THREE.Vector3();
      geometry.boundingBox.getSize(size);
      var maxDim = Math.max(size.x, size.y, size.z);
      var scale = 100 / maxDim;
      mesh.scale.set(scale, scale, scale);

      scene.add(mesh);
      camera.position.set(0, 60, 160);
      controls.target.set(0, 0, 0);
      controls.update();
    });

    function animate() {
      requestAnimationFrame(animate);
      controls.update();
      renderer.render(scene, camera);
    }
    animate();

    new ResizeObserver(function () {
      var w = container.clientWidth;
      camera.aspect = w / height;
      camera.updateProjectionMatrix();
      renderer.setSize(w, height);
    }).observe(container);
  });
});