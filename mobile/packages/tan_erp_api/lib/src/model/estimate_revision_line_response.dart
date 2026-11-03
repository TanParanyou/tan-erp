//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'estimate_revision_line_response.g.dart';

/// EstimateRevisionLineResponse
///
/// Properties:
/// * [sectionCode] 
/// * [workItemCode] 
/// * [descriptionTh] 
/// * [quantity] 
/// * [unitCode] 
/// * [totalCost] 
/// * [totalSellingPrice] 
@BuiltValue()
abstract class EstimateRevisionLineResponse implements Built<EstimateRevisionLineResponse, EstimateRevisionLineResponseBuilder> {
  @BuiltValueField(wireName: r'sectionCode')
  String get sectionCode;

  @BuiltValueField(wireName: r'workItemCode')
  String get workItemCode;

  @BuiltValueField(wireName: r'descriptionTh')
  String get descriptionTh;

  @BuiltValueField(wireName: r'quantity')
  double get quantity;

  @BuiltValueField(wireName: r'unitCode')
  String get unitCode;

  @BuiltValueField(wireName: r'totalCost')
  double get totalCost;

  @BuiltValueField(wireName: r'totalSellingPrice')
  double get totalSellingPrice;

  EstimateRevisionLineResponse._();

  factory EstimateRevisionLineResponse([void updates(EstimateRevisionLineResponseBuilder b)]) = _$EstimateRevisionLineResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(EstimateRevisionLineResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<EstimateRevisionLineResponse> get serializer => _$EstimateRevisionLineResponseSerializer();
}

class _$EstimateRevisionLineResponseSerializer implements PrimitiveSerializer<EstimateRevisionLineResponse> {
  @override
  final Iterable<Type> types = const [EstimateRevisionLineResponse, _$EstimateRevisionLineResponse];

  @override
  final String wireName = r'EstimateRevisionLineResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    EstimateRevisionLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'sectionCode';
    yield serializers.serialize(
      object.sectionCode,
      specifiedType: const FullType(String),
    );
    yield r'workItemCode';
    yield serializers.serialize(
      object.workItemCode,
      specifiedType: const FullType(String),
    );
    yield r'descriptionTh';
    yield serializers.serialize(
      object.descriptionTh,
      specifiedType: const FullType(String),
    );
    yield r'quantity';
    yield serializers.serialize(
      object.quantity,
      specifiedType: const FullType(double),
    );
    yield r'unitCode';
    yield serializers.serialize(
      object.unitCode,
      specifiedType: const FullType(String),
    );
    yield r'totalCost';
    yield serializers.serialize(
      object.totalCost,
      specifiedType: const FullType(double),
    );
    yield r'totalSellingPrice';
    yield serializers.serialize(
      object.totalSellingPrice,
      specifiedType: const FullType(double),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    EstimateRevisionLineResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required EstimateRevisionLineResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sectionCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.sectionCode = valueDes;
          break;
        case r'workItemCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.workItemCode = valueDes;
          break;
        case r'descriptionTh':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.descriptionTh = valueDes;
          break;
        case r'quantity':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(double),
          ) as double;
          result.quantity = valueDes;
          break;
        case r'unitCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(String),
          ) as String;
          result.unitCode = valueDes;
          break;
        case r'totalCost':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(double),
          ) as double;
          result.totalCost = valueDes;
          break;
        case r'totalSellingPrice':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(double),
          ) as double;
          result.totalSellingPrice = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  EstimateRevisionLineResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = EstimateRevisionLineResponseBuilder();
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


