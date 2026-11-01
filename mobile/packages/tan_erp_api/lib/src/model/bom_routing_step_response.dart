//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/work_center_ref_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'bom_routing_step_response.g.dart';

/// BomRoutingStepResponse
///
/// Properties:
/// * [id] 
/// * [sequence] 
/// * [operationCode] 
/// * [operationName] 
/// * [workCenter] 
/// * [setupHours] 
/// * [runHours] 
/// * [isSubcontracted] 
/// * [stdSubcontractCost] 
/// * [note] 
@BuiltValue()
abstract class BomRoutingStepResponse implements Built<BomRoutingStepResponse, BomRoutingStepResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'sequence')
  int? get sequence;

  @BuiltValueField(wireName: r'operationCode')
  String? get operationCode;

  @BuiltValueField(wireName: r'operationName')
  String? get operationName;

  @BuiltValueField(wireName: r'workCenter')
  WorkCenterRefResponse? get workCenter;

  @BuiltValueField(wireName: r'setupHours')
  double? get setupHours;

  @BuiltValueField(wireName: r'runHours')
  double? get runHours;

  @BuiltValueField(wireName: r'isSubcontracted')
  bool? get isSubcontracted;

  @BuiltValueField(wireName: r'stdSubcontractCost')
  double? get stdSubcontractCost;

  @BuiltValueField(wireName: r'note')
  String? get note;

  BomRoutingStepResponse._();

  factory BomRoutingStepResponse([void updates(BomRoutingStepResponseBuilder b)]) = _$BomRoutingStepResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BomRoutingStepResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BomRoutingStepResponse> get serializer => _$BomRoutingStepResponseSerializer();
}

class _$BomRoutingStepResponseSerializer implements PrimitiveSerializer<BomRoutingStepResponse> {
  @override
  final Iterable<Type> types = const [BomRoutingStepResponse, _$BomRoutingStepResponse];

  @override
  final String wireName = r'BomRoutingStepResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BomRoutingStepResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.sequence != null) {
      yield r'sequence';
      yield serializers.serialize(
        object.sequence,
        specifiedType: const FullType(int),
      );
    }
    if (object.operationCode != null) {
      yield r'operationCode';
      yield serializers.serialize(
        object.operationCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.operationName != null) {
      yield r'operationName';
      yield serializers.serialize(
        object.operationName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.workCenter != null) {
      yield r'workCenter';
      yield serializers.serialize(
        object.workCenter,
        specifiedType: const FullType(WorkCenterRefResponse),
      );
    }
    if (object.setupHours != null) {
      yield r'setupHours';
      yield serializers.serialize(
        object.setupHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.runHours != null) {
      yield r'runHours';
      yield serializers.serialize(
        object.runHours,
        specifiedType: const FullType(double),
      );
    }
    if (object.isSubcontracted != null) {
      yield r'isSubcontracted';
      yield serializers.serialize(
        object.isSubcontracted,
        specifiedType: const FullType(bool),
      );
    }
    if (object.stdSubcontractCost != null) {
      yield r'stdSubcontractCost';
      yield serializers.serialize(
        object.stdSubcontractCost,
        specifiedType: const FullType(double),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BomRoutingStepResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BomRoutingStepResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'sequence':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sequence = valueDes;
          break;
        case r'operationCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.operationCode = valueDes;
          break;
        case r'operationName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.operationName = valueDes;
          break;
        case r'workCenter':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(WorkCenterRefResponse),
          ) as WorkCenterRefResponse?;
          if (valueDes == null) continue;
          result.workCenter.replace(valueDes);
          break;
        case r'setupHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.setupHours = valueDes;
          break;
        case r'runHours':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.runHours = valueDes;
          break;
        case r'isSubcontracted':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isSubcontracted = valueDes;
          break;
        case r'stdSubcontractCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.stdSubcontractCost = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BomRoutingStepResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BomRoutingStepResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


