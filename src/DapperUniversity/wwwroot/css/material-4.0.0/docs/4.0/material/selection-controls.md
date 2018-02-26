---
layout: docs
title: Selection controls
description: Selection controls allow the user to select options.
group: material
toc: true
---

Three types of selection controls are covered in this guidance:

* **Checkboxes** allow the selection of multiple options from a set.
* **Radio buttons** allow the selection of a single option from a set.
* **Switches** allow a selection to be turned on or off.

## Checkboxes

**Most of the details about Material checkboxes have been covered in Components/Custom form elements docuementation. Please refer to [this page]({{ site.baseurl }}/docs/{{ site.docs_version }}/components/forms/#checkboxes) for more details.**

## Radio buttons

**Most of the details about Material radio buttons have been covered in Components/Custom form elements docuementation. Please refer to [this page]({{ site.baseurl }}/docs/{{ site.docs_version }}/components/forms/#radios) for more details.**

## Switches

Switches toggle the state of a single settings option. The option that the switch controls, as well as the state it’s in, should be made clear from the corresponding inline label.

{% example html %}
<div class="custom-control custom-switch">
  <input class="custom-control-input" id="customSwitch" type="checkbox">
  <span class="custom-control-track"></span>
  <label class="custom-control-label" for="customSwitch">Toggle this custom switch</label>
</div>
{% endexample %}
